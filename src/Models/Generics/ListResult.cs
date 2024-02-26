using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Models.Generics
{
	internal sealed record ListResult<T> : BaseResult
	{
		[NotNull]
		public IReadOnlyList<T> Items { get; set; } = [];

		public bool IsNotEmpty => Items is not null && Items.Any();

		public int Count => Items.Count;
	}
}