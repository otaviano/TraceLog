using System;
using System.Configuration;

namespace Otaviano.TraceLog.Persistencia.Factory
{
	static internal class FactoryFactoryDAO
	{
		public static IFactoryDAO GetFabrica()
		{
			if(string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("TipoProvedorDados")))
				throw new ArgumentNullException("TipoProvedorDados", "TipoProvedorDados não configurado no arquivo de configuração.");

			Conexao.TipoProvedorDados provider = (Conexao.TipoProvedorDados)Enum.Parse(typeof(Conexao.TipoProvedorDados), ConfigurationManager.AppSettings.Get("TipoProvedorDados"));

			switch (provider)
			{
				case Conexao.TipoProvedorDados.MSSQL:
					return new Factory.FactoryMSSQL();
				case Conexao.TipoProvedorDados.MySQL:
					return new Factory.FactoryMySQL();
				case Conexao.TipoProvedorDados.Odbc:
				case Conexao.TipoProvedorDados.OleDB:
				case Conexao.TipoProvedorDados.Oracle:
				case Conexao.TipoProvedorDados.Postgre:
				case Conexao.TipoProvedorDados.Sybase:
					throw new NotImplementedException("Provider Sybase não implementado");
				default:
					throw new ArgumentNullException("TipoProvedorDados", "TipoProvedorDados não implementado");
			}
		}
	}
}
