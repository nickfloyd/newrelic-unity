using System;
using UnityEngine;
using System.Collections.Generic;

namespace NewRelic
{
	public class NewRelicULinkInstrumentationBase : MonoBehaviour
	{
		private bool dontDestroyOnLoad = false;
		
		void Awake () {
			if (dontDestroyOnLoad) DontDestroyOnLoad(this);
		}
		
		void Start () {
			this.InitializeMetrics();
			StartCoroutines();
		}

		protected virtual void OnDisable() {
			StopAllCoroutines();
		}
		
		protected virtual void StartCoroutines() {}
		
		protected virtual void InitializeMetrics(){}
		
		public void SendMetrics(IReporter reporter, List<IMetric> metrics) {
			Debug.Log("Sending metrics to New Relic");
			try {
				reporter.Post(metrics);
				
			} catch (System.Exception ex) {
				Debug.LogException(ex);
			}
		}
	}
}

