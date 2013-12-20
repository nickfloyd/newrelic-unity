using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NewRelic
{
	[AddComponentMenu("New Relic/New Relic uLink Client Instrumentation")]
	public class NewRelicULinkClientInstrumentation : NewRelicULinkInstrumentationBase {
	
		private Metric frameRate = null;
		private Metric playerPing = null;
		public string licenseKey;
		
		protected override void OnDisable(){
			base.OnDisable();
			SendMetrics();
		}
		
		protected override void StartCoroutines() {
			
			base.StartCoroutines();
			
			//Start the harvest cycle for sending metrics
			StartCoroutine(Harvest(60.0F));
			
			//Start all time based aggregation and metrics gathering
			StartCoroutine(FrameRateCoroutine(1.0F));
			StartCoroutine(PlayerPingCoroutine(60.0F));
			//Add more Coroutines here

	    }
		
		protected override void InitializeMetrics(){
			base.InitializeMetrics();
			
			frameRate = new Metric { Name = "Client/fps[frames]" };
			playerPing = new Metric { Name = "Client/PlayerPing[ms]" };
		}
		
		private IEnumerator Harvest(float delayInSeconds) {
			
			Debug.Log("Starting harvest coroutine" + Time.time);
        	do {
				yield return new WaitForSeconds(delayInSeconds);
				try{
					SendMetrics();
		        } catch (System.Exception ex) {
					Debug.LogException(ex);	
				}	
			
			} while (true);
		}
		
		private IEnumerator FrameRateCoroutine(float delayInSeconds) {
			
			Debug.Log("Starting FrameRate coroutine" + Time.time);
			
				do {
					yield return new WaitForSeconds(delayInSeconds);
					try{
						if(frameRate != null){
							frameRate.Samples.Add(Mathf.RoundToInt(1.0f / Time.smoothDeltaTime));
						}
					} catch (System.Exception ex) {
						Debug.LogException(ex);
					}
				} while (true);
				

		}

		private IEnumerator PlayerPingCoroutine(float delayInSeconds) {
						
			Debug.Log("Starting Player Ping coroutine" + Time.time);

				do {
		        	yield return new WaitForSeconds(delayInSeconds);
					try {
						playerPing.Samples.Add(uLink.NetworkPlayer.server.averagePing);
					} catch (System.Exception ex) {
						Debug.LogException(ex);
					}
					
				} while (true);
			
		}
		
		private void SendMetrics() {
			Debug.Log("Sending metrics to New Relic");
			try {
				
				List<IMetric> metrics = new List<IMetric>();
				
				if (frameRate.Samples.Count > 0) {
					metrics.Add(frameRate);
				}
				if (playerPing.Samples.Count > 0) {
					metrics.Add(playerPing);
				}
				
				PlatformReporter reporter = new PlatformReporter(licenseKey, "Client", "1", "com.newrelic.unity.ulink.client", "Unity3D uLink Client");
				base.SendMetrics(reporter, metrics);
				
				this.InitializeMetrics();
			} catch (System.Exception ex) {
				Debug.LogException(ex);
			}
		}

	}
}

