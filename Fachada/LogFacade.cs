using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Otaviano.TraceLog.Dominio;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Otaviano.TraceLog.Fachada
{
	public class LogFacade
	{
		#region Propriedades

		#endregion

		#region Métodos

		/// <summary>
		/// Salva um exceção gerada pelo sistema.
		/// </summary>
		/// <param name="excecao">Exception recuperada</param>
		/// <param name="login">login do usuário logado quando ocorreu o erro.</param>
		/// <exception cref="ArgumentNullException">Ocorre quando um dos parametros de entrada são nulos.</exception>
		/// <exception cref="SystemException">Ocorre quando um erro inexperado é lançado (ver exceções anexas em InnerExpetion da exceção gerada).</exception>
		public static void RegistrarErro(Exception excecao, string login)
		{
			if (excecao == null)
				throw new ArgumentNullException("excecao", "parametro não pode ser nulo");
			if (string.IsNullOrEmpty(login))
				throw new ArgumentNullException("login", "parametro não pode ser nulo");

			try
			{
				Dominio.LogErro erro = new Dominio.LogErro(excecao, login);

				Persistencia.Factory.IFactoryDAO fabrica = Persistencia.Factory.FactoryFactoryDAO.GetFabrica();
				Persistencia.Abstracao.ILogDAO logAD = fabrica.GetLogAD();

				logAD.SalvarErro(erro);
			}
			catch (Exception ex)
			{
				throw new SystemException("Erro ao registrar erro", ex);
			}
		}
		/// <summary>
		/// Salva uma transação de banco (Insert, Update, Delete, Select).
		/// </summary>
		/// <param name="transacao">transação a ser registrada.</param>
		/// <exception cref="ArgumentNullException">Ocorre quando o objeto transação é nulo</exception>
		/// <exception cref="SystemException">Ocorre quando um erro inexperado é lançado (ver exceções anexas em InnerExpetion da exceção gerada).</exception>
		public static void RegistrarTransacao(Dominio.LogTransacao transacao)
		{
			if (transacao == null)
				throw new ArgumentNullException("transacao", "parametro não pode ser nulo");

			try
			{
				transacao.XmlVersao = LogFacade.SerializarXml(transacao.Versao);

				Persistencia.Factory.IFactoryDAO fabrica = Persistencia.Factory.FactoryFactoryDAO.GetFabrica();
				Persistencia.Abstracao.ILogDAO logAD = fabrica.GetLogAD();

				logAD.SalvarTransacao(transacao);
			}
			catch (Exception ex)
			{
				throw new SystemException("Erro ao registrar transação", ex);
			}
		}
		/// <summary>
		/// Salva um trace de depuração, ou qualquer informação pertinente para registro
		/// </summary>
		/// <param name="trace"></param>
		/// <exception cref="ArgumentNullException">Ocorre quando o objeto trace é nulo</exception>
		/// <exception cref="SystemException">Ocorre quando um erro inexperado é lançado (ver exceções anexas em InnerExpetion da exceção gerada).</exception>
		public static void RegistrarTrace(Dominio.LogTrace trace)
		{
			if (trace == null)
				throw new ArgumentNullException("trace", "parametro não pode ser nulo");

			try
			{
				Persistencia.Factory.IFactoryDAO fabrica = Persistencia.Factory.FactoryFactoryDAO.GetFabrica();
				Persistencia.Abstracao.ILogDAO logAD = fabrica.GetLogAD();

				logAD.SalvarTrace(trace);
			}
			catch (Exception ex)
			{
				throw new SystemException("Erro ao registrar trace", ex);
			}
		}
		/// <summary>
		/// Consulta de transações do sistema.
		/// </summary>
		/// <param name="transacao">objeto de filtros da pesquisa</param>
		/// <returns>lista de transações</returns>
		/// <exception cref="SystemException">Ocorre quando um erro inexperado é lançado (ver exceções anexas em InnerExpetion da exceção gerada).</exception>
		public static List<Dominio.LogTransacao> ConsultarTransacao(Dominio.LogTransacao transacao, Type tipoObjeto)
		{
			try
			{
				if (transacao == null)
					transacao = new Dominio.LogTransacao();

				Persistencia.Factory.IFactoryDAO fabrica = Persistencia.Factory.FactoryFactoryDAO.GetFabrica();
				Persistencia.Abstracao.ILogDAO logAD = fabrica.GetLogAD();

				List<Dominio.LogTransacao> listaTransacao = logAD.PesquisarTransacao(transacao);


				// Não está deserializando.
				// Deserializa todos os objetos para os quais foram gerados histórico.
				listaTransacao.ForEach
					(p => p.Versao = (p.XmlVersao != null) ? LogFacade.DeserializarXml(p.XmlVersao, tipoObjeto) : null);

				return listaTransacao;
			}
			catch (Exception ex)
			{
				throw new SystemException("Erro ao consultar transacao", ex);
			}
		}
		/// <summary>
		/// Consulta de erros do sistema.
		/// </summary>
		/// <param name="erro">objeto de filtros da pesquisa</param>
		/// <returns>Lista de erros</returns>
		/// <exception cref="SystemException">Ocorre quando um erro inexperado é lançado (ver exceções anexas em InnerExpetion da exceção gerada).</exception>
		public static List<Dominio.LogErro> ConsultarErro(Dominio.LogErro erro)
		{
			try
			{
				Persistencia.Factory.IFactoryDAO fabrica = Persistencia.Factory.FactoryFactoryDAO.GetFabrica();
				Persistencia.Abstracao.ILogDAO logAD = fabrica.GetLogAD();

				return logAD.PesquisarErro(erro);
			}
			catch (Exception ex)
			{
				throw new SystemException("Erro ao consultar erro", ex);
			}
		}
		/// <summary>
		/// Consulta de trace do sistema.
		/// </summary>
		/// <param name="trace">objeto de filtros da pesquisa</param>
		/// <returns>Lista de trace</returns>
		/// <exception cref="SystemException">Ocorre quando um erro inexperado é lançado (ver exceções anexas em InnerExpetion da exceção gerada).</exception>
		public static List<Dominio.LogTrace> ConsultarTrace(Dominio.LogTrace trace)
		{
			try
			{
				Persistencia.Factory.IFactoryDAO fabrica = Persistencia.Factory.FactoryFactoryDAO.GetFabrica();
				Persistencia.Abstracao.ILogDAO logAD = fabrica.GetLogAD();

				return logAD.PesquisarTrace(trace);
			}
			catch (Exception ex)
			{
				throw new SystemException("Erro ao consultar trace", ex);
			}
		}
		// serializa um objeto para xml.
		private static XmlDocument SerializarXml(object objeto)
		{
			try
			{
				XmlDocument xDocument = new XmlDocument();
				XmlSerializer serielizer = new XmlSerializer(objeto.GetType());

				using (MemoryStream stream = new MemoryStream())
				{
					serielizer.Serialize(stream, objeto);
					stream.Position = 0;

					xDocument.Load(stream);
				}

				return xDocument;
			}
			catch (IOException ex)
			{
				throw new SystemException("Erro ao gerar XML de Log", ex);
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// Desserializa um xml para o tipo informado
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="tipoObjeto"></param>
		/// <returns></returns>
		private static object DeserializarXml(XmlDocument xml, Type tipoObjeto)
		{
			try
			{
				XmlSerializer serielizer = new XmlSerializer(tipoObjeto);
				object objeto = null;

				using (XmlNodeReader reader = new XmlNodeReader(xml.DocumentElement))
				{
					objeto = serielizer.Deserialize(reader);
				}

				return objeto;
			}
			catch (IOException ex)
			{
				throw new SystemException("Erro ao gerar objeto de Log", ex);
			}
			catch
			{
				throw;
			}
		}

		#endregion
	}
}