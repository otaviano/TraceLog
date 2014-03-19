using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Otaviano.Conexao;
using System.Xml;
using MySql.Data.MySqlClient;

namespace Otaviano.TraceLog.Persistencia
{
	internal class LogMySQL : Abstracao.ILogDAO
	{
		private Conexao.Conexao objConexao;

		#region Métodos

		public LogMySQL()
		{
			objConexao = ConexaoFactory.GetConexao();
		}

		private Dominio.Log SalvarLog(Dominio.Log log)
		{
			string sql = string.Empty;

			try
			{
				sql = "INSERT INTO LOG_BASE (LOG_LOGIN, LOG_DATA)" +
							"VALUES(@LOG_LOGIN, CURDATE());" +
							"SELECT LAST_INSERT_ID();";

				IDataParameter[] parametros = new IDataParameter[]
				{
					new MySqlParameter("@LOG_LOGIN", log.Login)
				};

				log.Codigo = Convert.ToInt32(this.objConexao.ExecutarEscalar(sql, CommandType.Text, parametros));

				return log;
			}
			catch (MySqlException ex)
			{
				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		/// <summary>
		/// Persiste um log de erro.
		/// </summary>
		/// <param name="erro"></param>
		/// <param name="transacional">Desabilita operação transacional</param>
		public int SalvarErro(Dominio.LogErro erro, bool transacional = true)
		{
			string sql = string.Empty;

			try
			{
				if (transacional)
					this.objConexao.IniciarTransacao();

				erro.Codigo = this.SalvarLog(erro).Codigo;

				sql = "INSERT INTO LOG_ERRO(LOG_IDF, LOE_NOME_METODO, LOE_NOME_ENTIDADE, LOE_NOME_ERRO, LOE_DESCRICAO_ERRO, LOG_IDF_PAI)" +
							"VALUES (@LOG_IDF, @LOE_NOME_METODO, @LOE_NOME_ENTIDADE, @LOE_NOME_ERRO, @LOE_DESC_ERRO, @LOG_IDF_PAI)";

				IDataParameter[] parametros = new IDataParameter[]
		    {
		      new MySqlParameter("@LOG_IDF", erro.Codigo),
		      new MySqlParameter("@LOE_NOME_METODO", erro.NomeMetodo),
		      new MySqlParameter("@LOE_NOME_ENTIDADE", erro.NomeEntidade),
		      new MySqlParameter("@LOE_NOME_ERRO", erro.NomeErro),
		      new MySqlParameter("@LOE_DESC_ERRO", erro.DescricaoErro),
		      new MySqlParameter("@LOG_IDF_PAI", (erro.ErroPai != null) ? this.SalvarErro(erro.ErroPai, false) : (object) DBNull.Value)
		    };

				this.objConexao.ExecutarNonQuery(sql, CommandType.Text, parametros);

				if (transacional)
					this.objConexao.FinalizarTransacao();

				return erro.Codigo;
			}
			catch (MySqlException ex)
			{
				if (transacional)
					this.objConexao.CancelarTransacao();

				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				this.objConexao.CancelarTransacao();
				throw ex;
			}
		}
		/// <summary>
		/// Persiste um log de trace.
		/// </summary>
		/// <param name="trace"></param>
		public void SalvarTrace(Dominio.LogTrace trace, bool transacional = true)
		{
			string sql = string.Empty;

			try
			{
				if (transacional)
					this.objConexao.IniciarTransacao();

				trace.Codigo = this.SalvarLog(trace).Codigo;

				sql = "INSERT INTO LOG_TRACE(LOG_IDF, LOC_OBSERVACAO)" +
							"VALUES(@LOG_IDF, @LOC_OBSERVACAO)";
				
				IDataParameter[] parametros = new IDataParameter[]
				{
					new MySqlParameter("@LOG_IDF", trace.Codigo),
					new MySqlParameter("@LOC_OBSERVACAO", trace.Observacao),
		      };

				this.objConexao.ExecutarNonQuery(sql, CommandType.Text, parametros);

				if (transacional)
					this.objConexao.FinalizarTransacao();
			}
			catch (MySqlException ex)
			{
				if (transacional)
					this.objConexao.CancelarTransacao();

				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				this.objConexao.CancelarTransacao();
				throw ex;
			}
		}
		/// <summary>
		/// persiste o log de histórico de transações.
		/// </summary>
		/// <param name="transacao"></param>
		public void SalvarTransacao(Dominio.LogTransacao transacao, bool transacional = true)
		{
			string sql = string.Empty;

			try
			{
				if (transacional)
					this.objConexao.IniciarTransacao();

				transacao.Codigo = this.SalvarLog(transacao).Codigo;

				sql = @"INSERT INTO LOG_TRANSACAO (LOG_IDF, LON_VERSAO_ANTERIOR, LON_OBSERVACAO, LON_NOME_ENTIDADE, LON_DATA_VALIDADE, LON_IDF_ENTIDADE, TIO_IDF)
						VALUES(@LOG_IDF, @LON_VERSAO_ANTERIOR, @LON_OBSERVACAO, @LON_NOME_ENTIDADE, @LON_DATA_VALIDADE, @LON_IDF_ENTIDADE,@TIO_IDF)";

				IDataParameter[] parametros = new IDataParameter[]
		    {
				new MySqlParameter("@LOG_IDF", transacao.Codigo),
				new MySqlParameter("@LON_VERSAO_ANTERIOR", (transacao.XmlVersao != null)? transacao.XmlVersao.InnerXml : (object) DBNull.Value),
				new MySqlParameter("@LON_OBSERVACAO", (!string.IsNullOrEmpty(transacao.Observacao)) ? transacao.Observacao : (object) DBNull.Value),
				new MySqlParameter("@LON_NOME_ENTIDADE", (!string.IsNullOrEmpty(transacao.NomeEntidade) ? transacao.NomeEntidade : (object) DBNull.Value)),
				new MySqlParameter("@LON_DATA_VALIDADE", (transacao.DataValidade > DateTime.MinValue) ? transacao.DataValidade : (object) DBNull.Value),
				new MySqlParameter("@LON_IDF_ENTIDADE", transacao.CodigoEntidade),
				new MySqlParameter("@TIO_IDF", Convert.ToByte(transacao.Tipo))
		    };

				this.objConexao.ExecutarNonQuery(sql, CommandType.Text, parametros);

				if (transacional)
					this.objConexao.FinalizarTransacao();
			}
			catch (MySqlException ex)
			{
				if (transacional)
					this.objConexao.CancelarTransacao();

				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				this.objConexao.CancelarTransacao();
				throw ex;
			}
		}
		/// <summary>
		/// Retorna uma lista com os logs persistidos
		/// </summary>
		/// <param name="log"></param>
		/// <returns></returns>
		public List<Dominio.Log> PesquisarLog(Dominio.Log log)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Retorna uma lista com os logs de erro persistidos.
		/// </summary>
		/// <param name="erro"></param>
		/// <returns></returns>
		public List<Dominio.LogErro> PesquisarErro(Dominio.LogErro erro)
		{
			string sql = string.Empty;
			List<Dominio.LogErro> lista = new List<Dominio.LogErro>();
			IDataReader reader = null;

			try
			{
				sql = @"SELECT L.LOG_IDF, L.LOG_LOGIN, L.LOG_DATA, LOE_NOME_METODO, LOE_NOME_ENTIDADE, LOE_NOME_ERRO, LOE_DESCRICAO_ERRO, LOG_IDF_PAI
						FROM LOG_BASE L
						INNER JOIN LOG_ERRO E ON (E.LOG_IDF = L.LOG_IDF)
						WHERE (@LOG_IDF IS NULL OR L.LOG_IDF = @LOG_IDF)
						AND		(@LOE_NOME_ENTIDADE IS NULL OR LOE_NOME_ENTIDADE LIKE @LOE_NOME_ENTIDADE)
						AND		(@LOG_DATA IS NULL OR LOG_DATA = @LOG_DATA)
						AND		(@LOE_NOME_ERRO IS NULL OR LOE_NOME_ERRO LIKE @LOE_NOME_ERRO)
						AND		(@LOG_IDF_PAI IS NULL OR LOG_IDF_PAI = @LOG_IDF_PAI)";

				if (erro == null)
					erro = new Dominio.LogErro();

				IDataParameter[] parametros = new IDataParameter[]
				{
					new MySqlParameter("@LOG_IDF", (erro.Codigo > 0) ? erro.Codigo : (object) DBNull.Value),
					new MySqlParameter("@LOE_NOME_ENTIDADE", (!string.IsNullOrEmpty(erro.NomeEntidade)) ? erro.NomeEntidade : (object) DBNull.Value),
					new MySqlParameter("@LOG_DATA", (erro.Data > DateTime.MinValue) ? erro.Data : (object) DBNull.Value),
					new MySqlParameter("@LOE_NOME_ERRO", (!string.IsNullOrEmpty(erro.NomeErro) ? erro.NomeErro : (object) DBNull.Value)),
					new MySqlParameter("LOG_IDF_PAI", (erro.ErroPai != null) ? erro.ErroPai.Codigo : (object) DBNull.Value)
				};

				reader = this.objConexao.ExecutarDataReader(sql, CommandType.Text, parametros);

				while (reader.Read())
				{
					Dominio.LogErro obj = new Dominio.LogErro()
					{
						Codigo = Convert.ToInt32(reader["LOG_IDF"]),
						Login = Convert.ToString(reader["LOG_LOGIN"]),
						Data = Convert.ToDateTime(reader["LOG_DATA"]),
						NomeMetodo = Convert.ToString(reader["LOE_NOME_METODO"]),
						NomeEntidade = Convert.ToString(reader["LOE_NOME_ENTIDADE"]),
						NomeErro = Convert.ToString(reader["LOE_NOME_ERRO"]),
						DescricaoErro = Convert.ToString(reader["LOE_DESCRICAO_ERRO"]),
					};

					if (reader["LOG_IDF_PAI"] != DBNull.Value)
						obj.CodigoPai = Convert.ToInt32(reader["LOG_IDF_PAI"]);

					lista.Add(obj);
				}

				var folhas = from folha in lista
							 where folha.CodigoPai <= 0
							 select folha;

				foreach (var item in folhas)
				{
					var erroPai = from pai in lista
								  where pai.CodigoPai == item.Codigo
								  select pai;

					item.ErroPai = erroPai.First();
				}

				List<Dominio.LogErro> listaRetorno = folhas.ToList<Dominio.LogErro>();

				return listaRetorno;
			}
			catch (MySqlException ex)
			{
				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		/// <summary>
		/// Retorna uma lista com os logs de trace persistidos
		/// </summary>
		/// <param name="trace"></param>
		/// <returns></returns>
		public List<Dominio.LogTrace> PesquisarTrace(Dominio.LogTrace trace)
		{
			string sql = string.Empty;
			List<Dominio.LogTrace> lista = new List<Dominio.LogTrace>();

			try
			{
				sql = @"SELECT L.LOG_IDF,L.LOG_LOGIN,L.LOG_DATA, LOC_OBSERVACAO
								FROM LOG_BASE L
								INNER JOIN LOG_TRACE T ON (T.LOG_IDF = L.LOG_IDF)";

				IDataParameter[] parametros = new IDataParameter[]
				{
					new MySqlParameter("@LOG_IDF", trace.Codigo),
				};

				IDataReader reader = this.objConexao.ExecutarDataReader(sql, CommandType.Text, parametros);

				while (reader.Read())
				{
					Dominio.LogTrace obj = new Dominio.LogTrace()
					{
						Codigo = Convert.ToInt32(reader["LOG_IDF"]),
						Login = Convert.ToString(reader["LOG_LOGIN"]),
						Data = Convert.ToDateTime(reader["LOG_DATA"]),
						Observacao = Convert.ToString(reader["LOC_OBSERVACAO"])
					};

					lista.Add(obj);
				}

				return lista;
			}
			catch (MySqlException ex)
			{
				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		/// <summary>
		/// Retorna uma lista com os logs de transação persistidos
		/// </summary>
		/// <param name="transacao"></param>
		/// <returns></returns>
		public List<Dominio.LogTransacao> PesquisarTransacao(Dominio.LogTransacao transacao)
		{
			string sql = string.Empty;
			List<Dominio.LogTransacao> lista = new List<Dominio.LogTransacao>();

			try
			{
				sql = @"SELECT  L.LOG_IDF, L.LOG_LOGIN, L.LOG_DATA, T.LON_NOME_ENTIDADE, T.LON_VERSAO_ANTERIOR, 
								T.LON_OBSERVACAO, T.LON_DATA_VALIDADE, LON_IDF_ENTIDADE, TI.TIO_IDF
						FROM LOG_BASE L
						INNER JOIN LOG_TRANSACAO T ON (T.LOG_IDF = L.LOG_IDF)
						INNER JOIN TIPO_OPERACAO TI ON (TI.TIO_IDF = T.TIO_IDF)
						WHERE	(@LOG_IDF IS NULL OR L.LOG_IDF = @LOG_IDF)
						AND		(@LON_IDF_ENTIDADE IS NULL OR LON_IDF_ENTIDADE = @LON_IDF_ENTIDADE)
						AND		(@LON_NOME_ENTIDADE IS NULL OR LON_NOME_ENTIDADE LIKE @LON_NOME_ENTIDADE)";

				IDataParameter[] parametros = new IDataParameter[]
				{
					new MySqlParameter("@LOG_IDF", (transacao.Codigo > 0) ? transacao.Codigo : (object) DBNull.Value),
					new MySqlParameter("@LON_IDF_ENTIDADE", (transacao.CodigoEntidade > 0) ? transacao.CodigoEntidade : (object) DBNull.Value),
					new MySqlParameter("@LON_NOME_ENTIDADE", (!string.IsNullOrEmpty(transacao.NomeEntidade)) ? transacao.NomeEntidade : (object) DBNull.Value)
				};

				IDataReader reader = this.objConexao.ExecutarDataReader(sql, CommandType.Text, parametros);

				while (reader.Read())
				{
					XmlDocument xml = new XmlDocument();

					if (reader["LON_VERSAO_ANTERIOR"] != DBNull.Value)
						xml.LoadXml(Convert.ToString(reader["LON_VERSAO_ANTERIOR"]));

					Dominio.LogTransacao obj = new Dominio.LogTransacao()
					{
						Codigo = Convert.ToInt32(reader["LOG_IDF"]),
						Login = Convert.ToString(reader["LOG_LOGIN"]),
						Data = Convert.ToDateTime(reader["LOG_DATA"]),
						NomeEntidade = Convert.ToString(reader["LON_NOME_ENTIDADE"]),
						XmlVersao = xml,
						Observacao = Convert.ToString(reader["LON_OBSERVACAO"]),
						CodigoEntidade = Convert.ToInt32(reader["LON_IDF_ENTIDADE"]),
						Tipo = (Dominio.TipoOperacao)Convert.ToInt32(reader["TIO_IDF"])
					};

					if (reader["LON_DATA_VALIDADE"] != DBNull.Value)
						obj.DataValidade = Convert.ToDateTime(reader["LON_DATA_VALIDADE"]);

					lista.Add(obj);
				}

				return lista;
			}
			catch (MySqlException ex)
			{
				throw new ApplicationException("Ocorreu um erro ao acessar o banco de dados!", ex);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		/// <summary>
		/// Exclui os logs com a validade de persistencia vencida,
		/// caso a mesma tenha sido informada ao registrar o log.
		/// </summary>
		public void ExcluirLogsExpirados()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Cria tabelas e relacionamentos do banco.
		/// </summary>
		public void ConstruirEstruturaLog()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Carrega as execeções filhas 
		/// </summary>
		/// <param name="erro"></param>
		private void CarregarErroFilho(Dominio.LogErro erro)
		{
			Dominio.LogErro erroParam = new Dominio.LogErro()
			{
				ErroPai = new Dominio.LogErro() { Codigo = erro.Codigo }
			};

			List<Dominio.LogErro> lista = this.PesquisarErro(erroParam);

			if (lista.Count <= 0)
				return;

			Dominio.LogErro filho = lista[0];
			filho.ErroPai = erro;

			this.CarregarErroFilho(filho);
		}

		#endregion
	}
}
