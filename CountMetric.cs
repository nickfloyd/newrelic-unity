using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NewRelic
{
	public class CountMetric : IMetric
	{
		private int count = 0;
		
		public int Count {
			get{
				return count;
			}
			set{
				count = value;
			}
		}
		
		public CountMetric (){}
		
		public void Increment(){
			count++;
		}
		
		public void Decrement(){
			count--;
		}
		
		public string Name { get; set; }
		
		public string MetricName(){
			return string.Format("Component/Unity/{0}",this.Name);
		}
		
		public float Value() {
			return (float) count;
		}
		
		public string ToJson() {
			string json = string.Format("[{0},{1},{2},{3},{4}]",
				count, 1, count, count, 0);
			Debug.Log(json);
			
			return json;
		}

	}
}

