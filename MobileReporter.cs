using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine;

namespace NewRelic {
	
	public class MobileReporter : IReporter {
		private string host = "http://mobile-collector.newrelic.com/mobile/v1/";
		private string appToken;
		
		public MobileReporter (string appToken) {
			this.appToken = appToken;
			ConnectPost();
		}
		
		public void Post(List<IMetric> metrics) {
			DataPost(metrics);
		}
		
		private void ConnectPost() {
			string endPoint = host + "connect";
			string json = "[[\"EchoCamp\",\"0.0.1\",\"com.newrelic.EchoCamp\"],[\"iOS\",\"6.0\",\"iPhone 4\",\"iOSAgent\",\"0.2.0\",\"XXX\"]]";
			HttpPost(endPoint, json);
		}
		
		private void DataPost(List<IMetric> metrics) {
			string endPoint = host + "data";
			string json = "[]";
			HttpPost(endPoint, json);
		}
		
		private void HttpPost(string endPoint, string json) {
			try {
				WebRequest request = WebRequest.Create(endPoint);
				request.Headers.Add ("X-App-License-Key", appToken);
				request.ContentType = "application/json";
				request.Method = "POST";
				
				byte[] byteArray = Encoding.UTF8.GetBytes(json);
				using (Stream dataStream = request.GetRequestStream()){
				    dataStream.Write (byteArray, 0, byteArray.Length);
		        
					using(WebResponse response = request.GetResponse()){
						var statusCode = ((HttpWebResponse)response).StatusCode;
						if(response != null && statusCode >= HttpStatusCode.BadRequest){
							Debug.LogError(string.Format("Unable to send metrics to New Relic! Http Status Code: {0}", statusCode));
						}
						else{
							using(StreamReader reader = new StreamReader (response.GetResponseStream())){
								string responseBody = reader.ReadToEnd(); // {"data_token":"ogiwharworsltkjaz"}
								Debug.Log(string.Format("Metric post was successful! Response from server: {0}", responseBody));
								
							}							
						}
					}
				}
			} catch (Exception ex) {
				Debug.LogError(ex.Message);	
			}
		}
	}
}

