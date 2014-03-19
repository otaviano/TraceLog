using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otaviano.TraceLog.Dominio
{
	public class LogErro : Log
	{
		#region Propriedades

		public string NomeMetodo{ get; internal set; }
		/// <summary>
		/// Exceção lançada
		/// </summary>
		public Exception Excecao { get; internal set; }
		/// <summary>
		/// nome da classe, subSistema ou tela em que ocorreu o erro.
		/// </summary>
		public string NomeEntidade { get; internal set; }
		/// <summary>
		/// nome da exceção recebida.
		/// </summary>
		public string NomeErro { get; internal set; }
		/// <summary>
		/// StackTrace da exceção.
		/// </summary>
		public string DescricaoErro { get; internal set; }
		/// <summary>
		/// Mensagem de erro da exceção.
		/// </summary>
		public string Mensagem { get; internal set; }

		internal int CodigoPai { get; set; }

		public LogErro ErroPai { get; set; }

		#endregion

		public LogErro(Exception excecao, string login)
		{
			this.Excecao	= excecao;
			
			this.Login		= login;
			this.Data			= DateTime.Now;

			this.CarregarParametros();
			this.CarregarErroPai(this);
		}
		
		public LogErro()
		{

		}

		private void CarregarParametros()
		{
			if (this.Excecao != null)
			{
				this.NomeMetodo			= this.Excecao.TargetSite.Name;
				this.NomeErro				= this.Excecao.GetType().Name;
				this.NomeEntidade		= this.Excecao.TargetSite.DeclaringType.ToString();
				this.DescricaoErro	= this.Excecao.StackTrace;
				this.Mensagem				= this.Excecao.Message;
			}
		}

		private void CarregarErroPai(LogErro erro)
		{
			if(this.Excecao.InnerException != null)
			{
				Exception excecaoPai = this.Excecao.InnerException;
				this.ErroPai = new LogErro(excecaoPai, this.Login);
			}

			return;
		}

		public override string ToString()
		{
			return this.Mensagem;
		}
	}
}
