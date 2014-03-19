using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otaviano.TraceLog.Dominio
{
	public abstract class Log
	{
		public int Codigo { get; set; }

		/// <summary>
		/// Login do usuário autenticado quando o log foi registrado
		/// </summary>
		public string Login { get; set; }

		public DateTime Data 
		{ 
			get; internal set;
		}
	}
}
