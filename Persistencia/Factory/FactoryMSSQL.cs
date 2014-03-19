
namespace Otaviano.TraceLog.Persistencia.Factory
{
	internal class FactoryMSSQL : IFactoryDAO
	{
		public Abstracao.ILogDAO GetLogAD()
		{
			return new LogMSSQL();
		}
	}
}
