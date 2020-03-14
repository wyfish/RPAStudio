using System;
using System.Runtime.Serialization;

namespace RPA.Core.Activities
{
	[Serializable]
	public class CheckpointException : Exception
	{
		public CheckpointException()
		{
		}
		public CheckpointException(string message) : base(message)
		{
		}
		protected CheckpointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
