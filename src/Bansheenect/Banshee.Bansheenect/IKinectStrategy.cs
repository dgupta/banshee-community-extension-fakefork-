using System;
namespace Banshee.Bansheenect
{
	public interface IKinectStrategy
	{
		void HandlePush(float velocity, float angle);
		
		//swipes
		void HandleSwipeLeft(float left, float right);
		void HandleSwipeRight(float left,float right);
		void HandleSwipeUp(float left,float right);
		void HandleSwipeDown(float left,float right);
		
		//circles
		void HandlePositiveCircle();
		void HandleNegativeCircle();
	
	
}

