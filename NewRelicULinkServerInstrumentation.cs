using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NewRelic;
using System.Net;
using System.IO;

namespace NewRelic {

	[AddComponentMenu("New Relic/New Relic uLink Server Instrumentation")]
	public class NewRelicULinkServerInstrumentation : NewRelicULinkInstrumentationBase {
		
		private Metric frameRate = null;
		private Metric playerPing = null;
		private CountMetric playerCount = null;
		
		public string licenseKey; //b0a13eb6f039a6ca9b1c887181a1927b20d10faf
		
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
			
			frameRate = new Metric { Name = "Server/fps[frames]" };
			playerPing = new Metric { Name = "Server/PlayerPing[ms]" };
			playerCount = new CountMetric { Name = "Server/Players[count]" };
		}
		
		private int PlayerCount {
			get{
				return uLink.Network.isServer ? uLink.Network.connections.Length : 0;
			}
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
						uLink.NetworkPlayer[] connections = uLink.Network.connections;
						
						Debug.Log(string.Format("Running Player Ping CR with values - playerPing:{0}, uLink.Network.isServer:{1}, this.PlayerCount:{2}",playerPing.Samples.Count, uLink.Network.isServer, this.PlayerCount));
						
						if(playerPing != null 
							&& uLink.Network.isServer
							&& this.PlayerCount > 0){
							
							int avgPing = 0;
							foreach (uLink.NetworkPlayer player in connections){
								avgPing += player.averagePing;
							}
							
							if(avgPing > 0){
								playerPing.Samples.Add(Mathf.RoundToInt(avgPing / this.PlayerCount));
							}
						}
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
				
				playerCount.Count = PlayerCount;
				metrics.Add(playerCount);
				
				PlatformReporter reporter = new PlatformReporter(licenseKey, uLink.MasterServer.ipAddress, "1", "com.newrelic.unity.ulink.server", "Unity3D uLink Server");
				
				
				base.SendMetrics(reporter, metrics);
				
				this.InitializeMetrics();
			} catch (System.Exception ex) {
				Debug.LogException(ex);
			}
		}

	}
}
