using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otaviano.TraceLog.Persistencia.Abstracao
{
	interface ILogDAO
	{
		#region Métodos

		// Cadastro / Alteracao
		int SalvarErro(Dominio.LogErro erro, bool transacional = true);
		void SalvarTrace(Dominio.LogTrace trace, bool transacional = true);
		void SalvarTransacao(Dominio.LogTransacao transacao, bool transacional = true);
		
		// Pesquisa
		List<Dominio.Log> PesquisarLog(Dominio.Log log);
		List<Dominio.LogErro> PesquisarErro(Dominio.LogErro Erro);
		List<Dominio.LogTrace> PesquisarTrace(Dominio.LogTrace trace);
		List<Dominio.LogTransacao> PesquisarTransacao(Dominio.LogTransacao transacao);

		void ExcluirLogsExpirados();
		void ConstruirEstruturaLog();

		#endregion
	}
}
