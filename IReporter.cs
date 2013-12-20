using System;
using System.Collections.Generic;

namespace NewRelic
{
	public interface IReporter
	{
		void Post(List<IMetric> metrics);
	}
}
