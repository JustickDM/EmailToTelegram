namespace EmailToTelegram.Events
{
	internal sealed class ItemEventArgs<T> : EventArgs where T : class
	{
		public required T Item { get; set; }
	}
}