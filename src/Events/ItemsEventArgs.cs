namespace EmailToTelegram.Events
{
	internal sealed class ItemsEventArgs<T> : EventArgs where T : class
	{
		public required IReadOnlyList<T> Items { get; set; }
	}
}