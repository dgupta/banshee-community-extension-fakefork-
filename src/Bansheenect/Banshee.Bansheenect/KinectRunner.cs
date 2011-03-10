using System;
using xn;
using xnv;

namespace Banshee.Bansheenect
{
	public class KinectRunner
	{
		public KinectRunner ()
		{
		}
		
		void Init(){
			Setup();
		}
		
		void Setup ()
		{
			//build the context
			Context = new Context(CONFIG);
			
			//build the session manager
			SeshManager = new SessionManager(Context,"RaiseHand","RaiseHand");
			SeshManager.SetQuickRefocusTimeout(15000);
			
			//build the flow router
			Flowy = new FlowRouter();
			
			//build the Broadcaster
			
			                               
		}

		//setup fields 
		//initializations
		private readonly string CONFIG = @"../../config.xml";
		//a nice list of readonly strings
		private readonly string SWIPEUP = "SwipeUp", SWIPELEFT ="SwipeLeft",SWIPERIGHT = "SwipeRight", SWIPEDOWN = "SwipeDown", ONPUSH = "OnPush";
		
		
		Context Context;
		//Managers
		SessionManager SeshManager;
		FlowRouter Flowy;
		Broadcaster BCaster;
		//Detectors
		PushDetector Pushy;
		SwipeDetector Swipy;
	}
}

