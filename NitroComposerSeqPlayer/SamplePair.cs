using System;
using System.Collections.Generic;

namespace Henke37.Nitro.Composer.Player {
	public struct SamplePair : IEquatable<SamplePair>, IEquatable<int> {
		public int Left;
		public int Right;

		public SamplePair(int val) {
			Left = val;
			Right = val;
		}

		public SamplePair(int left, int right) {
			Left = left;
			Right = right;
		}

		public int Mono { get => (Left + Right) / 2; }

		public int this[int i] {
			get {
				switch(i) {
					case 0: return Left; ;
					case 1: return Right;
				}
				throw new ArgumentOutOfRangeException("value");
			}
			set {
				switch(i) {
					case 0: Left = value; return;
					case 1: Right = value; return;
				}
				throw new ArgumentOutOfRangeException("value");
			}
		}

		public bool Equals(SamplePair other) {
			return other.Left == Left && other.Right == Right;
		}

		public bool Equals(int other) {
			if(Left != Right) return false;
			return Left == other;
		}

		public override bool Equals(object obj) {
			if(obj is SamplePair otherPair) {
				return Equals(otherPair);
			}
			if(obj is int otherInt) {
				return Equals(otherInt);
			}
			return false;
		}

		public override int GetHashCode() {
			var hashCode = -1051820395;
			hashCode = hashCode * -1521134295 + Left.GetHashCode();
			hashCode = hashCode * -1521134295 + Right.GetHashCode();
			return hashCode;
		}

		public override string ToString() {
			return $"{Left} {Right}";
		}

		public static bool operator ==(SamplePair left, SamplePair right) {
			return left.Equals(right);
		}

		public static bool operator !=(SamplePair left, SamplePair right) {
			return !left.Equals(right);
		}

		public static SamplePair operator +(SamplePair left, SamplePair right) {
			left.Left += right.Left;
			left.Right += right.Right;
			return left;
		}
		public static SamplePair operator -(SamplePair left, SamplePair right) {
			left.Left -= right.Left;
			left.Right -= right.Right;
			return left;
		}
		public static SamplePair operator +(SamplePair left, int right) {
			left.Left += right;
			left.Right += right;
			return left;
		}
		public static SamplePair operator +(int left, SamplePair right) {
			right.Left += left;
			right.Right += left;
			return right;
		}
		public static SamplePair operator -(SamplePair left, int right) {
			left.Left -= right;
			left.Right -= right;
			return left;
		}
		public static SamplePair operator -(int left, SamplePair right) {
			right.Left = left - right.Left;
			right.Right = left - right.Right;
			return right;
		}

		public static SamplePair operator *(SamplePair left, int right) {
			left.Left *= right;
			left.Right *= right;
			return left;
		}
		public static SamplePair operator *(int left, SamplePair right) {
			right.Left *= left;
			right.Right *= left;
			return right;
		}

		public static SamplePair operator /(SamplePair left, int right) {
			left.Left /= right;
			left.Right /= right;
			return left;
		}
		public static SamplePair operator %(SamplePair left, int right) {
			left.Left %= right;
			left.Right %= right;
			return left;
		}

		public static SamplePair operator <<(SamplePair left, int right) {
			left.Left <<= right;
			left.Right <<= right;
			return left;
		}
		public static SamplePair operator >>(SamplePair left, int right) {
			left.Left >>= right;
			left.Right >>= right;
			return left;
		}

		public static SamplePair operator &(SamplePair left, int right) {
			left.Left &= right;
			left.Right &= right;
			return left;
		}
		public static SamplePair operator &(int left, SamplePair right) {
			right.Left &= left;
			right.Right &= left;
			return right;
		}
		public static SamplePair operator |(SamplePair left, int right) {
			left.Left |= right;
			left.Right |= right;
			return left;
		}
		public static SamplePair operator |(int left, SamplePair right) {
			right.Left |= left;
			right.Right |= left;
			return right;
		}

		public static SamplePair operator ~(SamplePair val) {
			val.Left = ~val.Left;
			val.Right = ~val.Right;
			return val;
		}

		public static explicit operator bool(SamplePair val) {
			return val.Left != 0 && val.Right != 0;
		}
		public static explicit operator int(SamplePair val) {
			return val.Mono;
		}

		public static implicit operator SamplePair(int val) {
			return new SamplePair(val);
		}
	}
}
