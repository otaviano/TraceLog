using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otaviano.TraceLog.Dominio
{
	public enum TipoOperacao : byte
	{
		Cadastro		= 1,
		Alteracao		= 2,
		Exclusao		= 3,
		Visualizacao	= 4
	}
}
