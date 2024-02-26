using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Models.Generics
{
	internal sealed record SingleResult<T> : BaseResult
	{
		[MaybeNull]
		public T? Item { get; set; } = default;

		public bool IsNotEmpty => Item is not null;
	}
}