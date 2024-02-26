using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Models.Generics
{
	internal record BaseResult
	{
		[MaybeNull]
		public Exception? Exception { get; set; }

		public bool IsSuccess => Exception is null;
	}
}