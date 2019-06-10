using System;
using System.Collections.Generic;

namespace Henke37.Nitro.Graphics.Animation {
	public abstract class AnimationRunner {
		protected NANR.Animation animation;
		protected NCGR ncgr;
		protected NCLR nclr;
		protected NCER ncer;

		protected int currentAnimationFrameIndex;
		protected int animationTimeout;
		protected int animationDirection = 1;

		public AnimationRunner(NANR.Animation animation, NCGR ncgr, NCLR nclr, NCER ncer) {
			this.animation = animation;
			this.ncgr = ncgr;
			this.nclr = nclr;
			this.ncer = ncer;
		}

		public void tick() {
			if(animationTimeout==0) {
				nextFrame();
			} else {
				animationTimeout--;
			}
		}

		protected NANR.Animation.AnimationFrame currentAnimationFrame { get => animation.Frames[currentAnimationFrameIndex]; }

		protected void nextFrame() {
			if(animationDirection>0) {
				if(currentAnimationFrameIndex + 1 >= animation.Frames.Count) {
					switch(animation.PlaybackMode) {
						case NANR.Animation.AnimationPlaybackMode.Forward:
							OnAnimationComplete();
							return;
						case NANR.Animation.AnimationPlaybackMode.Forward_Loop:
							currentAnimationFrameIndex = animation.LoopStart-1;
							break;
						case NANR.Animation.AnimationPlaybackMode.PingPong_Once:
						case NANR.Animation.AnimationPlaybackMode.PingPong_Loop:
							animationDirection = -1;
							break;
						default:
							throw new NotSupportedException();
					}
				}
			} else {//going in reverse
				if(currentAnimationFrameIndex-1 <= animation.LoopStart) {
					switch(animation.PlaybackMode) {
						case NANR.Animation.AnimationPlaybackMode.PingPong_Once:
							OnAnimationComplete();
							return;
						case NANR.Animation.AnimationPlaybackMode.PingPong_Loop:
							animationDirection = 1;
							break;
						default:
							throw new NotSupportedException();
					}
				}
			}
			currentAnimationFrameIndex += animationDirection;
			animationTimeout = currentAnimationFrame.FrameTime;
			drawNewFrame();
		}

		protected abstract void OnAnimationComplete();
		protected abstract void drawNewFrame();
	}
}
