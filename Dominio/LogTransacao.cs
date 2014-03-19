using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Otaviano.TraceLog.Dominio
{
	public class LogTransacao : Log
	{
		/// <summary>
		/// Registra o tipo de transação realizada pelo usuário.
		/// Insert | Update | Delete | Select
		/// </summary>
		public TipoOperacao Tipo { get; set; }
		/// <summary>
		/// Objeto com versão da alteração
		/// </summary>
		public object Versao { get; set; }
		/// <summary>
		///  Persiste o estado anterior da entidade, armazenando os valores no formato XML.
		/// </summary>
		/// <remarks>
		///  Deve ser usado com caltela para não sobrecarregar o banco com excesso de registros
		/// </remarks>
		internal XmlDocument XmlVersao { get; set; }
		/// <summary>
		/// Nome do objeto ou subsistema ao qual o log pertence.
		/// </summary>
		public string NomeEntidade { get; set; }
		/// <summary>
		/// Define uma data em que o registro estará disponivel no banco.
		/// Ao expirar a data o registro será excluido.
		/// </summary>
		public DateTime DataValidade { get; set; }
		/// <summary>
		/// Descrições adicionais.
		/// </summary>
		public string Observacao { get; set; }
		/// <summary>
		/// Codigo para referencia do item logado.
		/// </summary>
		public int CodigoEntidade { get; set; }
	}
}
