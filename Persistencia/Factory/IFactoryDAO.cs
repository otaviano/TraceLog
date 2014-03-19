using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otaviano.TraceLog.Persistencia.Factory
{
	internal interface IFactoryDAO
	{
		Abstracao.ILogDAO GetLogAD();
	}
}
