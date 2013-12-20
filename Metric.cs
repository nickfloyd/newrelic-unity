using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NewRelic
{
	public class Metric : IMetric
	{
		private List<float> samples;
		
		public Metric (){}
		
		public string Name { get; set; }
		
		public string MetricName(){
			return string.Format("Component/Unity/{0}",this.Name);
		}
		
		public float Value() {
			return samples.Average();	
		}
		
		public List<float> Samples {
			get {
				if (samples == null) samples = new List<float>();
				return samples;
			}
			
			set {samples = value;}
		}
		
		public float Min {
			get{ 
				return Enumerable.Min(samples);
			}
		}
		
		public float Max {
			get{ 
				return Enumerable.Max(samples);
			}
		}
		
		public int SumOfSquares {
			get{ 
				return GetSumOfSquares(this.Samples);
			}
		}
		
		private static int GetSumOfSquares(ICollection<float> list) {
			int sos = 0;
        	if (list != null && list.Count > 0) {
        		sos = list.Select(x => (int)x * (int)x).Sum();
			}
			return sos;
		}
		
		public string ToJson() {
			string json = string.Format("[{0},{1},{2},{3},{4}]",
				samples.Average(), 1, this.Min, this.Max, this.SumOfSquares);
			Debug.Log(json);
			return json;
		}

	}
}

