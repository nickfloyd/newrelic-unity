using System;

namespace NewRelic {
	public interface IMetric {
		string MetricName();
		float Value();
		string ToJson();
	}
}

