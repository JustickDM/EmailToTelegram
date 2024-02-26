using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EmailToTelegram.Extentions
{
	internal static class ArgumentExceptionExtentions
	{
		public static T ThrowIfNull<T>(
			[NotNull] this T? argument,
			[CallerMemberName] string? method = null,
			[CallerArgumentExpression(nameof(argument))] string? argumentName = null)
		{
			if (argument is null)
				ThrowArgumentNullException(argumentName, method);

			return argument;
		}

		public static IEnumerable<T> ThrowIfNullOrNotAny<T>(
			[NotNull] this IEnumerable<T>? argument,
			[CallerMemberName] string? method = null,
			[CallerArgumentExpression(nameof(argument))] string? argumentName = null)
		{
			if (argument is null || !argument.Any())
				ThrowArgumentNullException(argumentName, method);

			return argument;
		}

		public static string ThrowIfNullOrWhiteSpace(
			[NotNull] this string? argument,
			[CallerMemberName] string? method = null,
			[CallerArgumentExpression(nameof(argument))] string? argumentName = null)
		{
			if (string.IsNullOrWhiteSpace(argument))
				ThrowArgumentNullException(argumentName, method);

			return argument;
		}

		public static int ThrowIfLessThanOrEqualZero(
			[NotNull] this int argument,
			[CallerMemberName] string? method = null,
			[CallerArgumentExpression(nameof(argument))] string? argumentName = null)
		{
			if (argument <= default(int))
				ThrowArgumentNullException(argumentName, method);

			return argument;
		}

		[DoesNotReturn]
		private static void ThrowArgumentNullException(string? argumentName, string? method)
			=> throw new ArgumentNullException(string.Format("Argument: {0} | Method: {1}", argumentName, method));
	}
}