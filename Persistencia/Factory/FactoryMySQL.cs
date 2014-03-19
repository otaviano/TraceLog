using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otaviano.TraceLog.Persistencia.Factory
{
	internal class FactoryMySQL : IFactoryDAO
	{
		public Abstracao.ILogDAO GetLogAD()
		{
			return new LogMySQL();
		}
	}
}
