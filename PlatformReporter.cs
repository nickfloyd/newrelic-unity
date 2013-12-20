using System;
using System.Net;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NewRelic
{
	public class PlatformReporter : IReporter
	{
		private string endPoint = "http://platform-api.newrelic.com/platform/v1/metrics";
		private string licenseKey;
		private string host = "server";
		private string processID = "1";
		private string guid = "com.newrelic.unity.ulink.server";
		private string componentName = "Unity3D uLink Server";
		
		public PlatformReporter(string licenseKey, string host = "", string processID = "", string guid="", string componentName="") {
			this.licenseKey = licenseKey;
			
			if(!string.IsNullOrEmpty(host)){
				this.host = host;
			}
			
			if(!string.IsNullOrEmpty(processID)){
				this.processID = processID;
			}
			
			if(!string.IsNullOrEmpty(guid)){
				this.guid = guid;
			}
			
			if(!string.IsNullOrEmpty(componentName)){
				this.componentName = componentName;
			}
			
		}
		

		
		public void Post(List<IMetric> metrics) {
			try {
				
				WebRequest request = WebRequest.Create(endPoint);
	            
				string json = "{\"agent\":{\"host\":\"" + host + "\",\"pid\":" + processID + ",\"version\":\"0.0.1\"},\"components\":[{\"name\":\"" + componentName + "\",\"guid\":\"" + guid + "\",\"duration\":60,\"metrics\":{";
				json += string.Join(",", metrics.Select(m => string.Format("\"{0}\":{1}",m.MetricName(),m.ToJson())).ToArray());
				json += "}}]}";
				
				Debug.Log("Harvest json: " + json);
				
				byte[] byteArray = Encoding.UTF8.GetBytes (json);
				request.Headers.Add ("X-License-Key", licenseKey);
	            request.ContentType = "application/json";
				request.Method = "POST";
	            request.ContentLength = byteArray.Length;
	            
				using (Stream dataStream = request.GetRequestStream()){
				    dataStream.Write (byteArray, 0, byteArray.Length);
		        
					using(WebResponse response = request.GetResponse()){
						var statusCode = ((HttpWebResponse)response).StatusCode;
						if(response != null && statusCode >= HttpStatusCode.BadRequest){
							Debug.LogError(string.Format("Unable to send metrics to New Relic! Http Status Code: {0}", statusCode));
						}
						else{
							using(StreamReader reader = new StreamReader (response.GetResponseStream())){
								Debug.Log(string.Format("Metric post was successful! Response from server: {0}", reader.ReadToEnd()));
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

