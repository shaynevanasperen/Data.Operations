using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Magneto.Core
{
	/// <summary>
	/// A wrapper class for holding cached values (so that we can cache nulls).
	/// </summary>
	/// <typeparam name="T">The type of value being cached.</typeparam>
	public sealed class CacheEntry<T> : IEquatable<CacheEntry<T>>
	{
		/// <summary>
		/// Creates a new <see cref="CacheEntry{T}"/> wrapping the given <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value to wrap.</param>
		public CacheEntry(T value) => Value = value;

		/// <summary>
		/// The cached value.
		/// </summary>
		public T Value { get; }

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			return obj is CacheEntry<T> other && Equals(other);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(Value);
		}

		/// <inheritdoc />
		public bool Equals(CacheEntry<T> other)
		{
			if (other is null)
				return false;

			return ReferenceEquals(this, other) ||
				   EqualityComparer<T>.Default.Equals(Value, other.Value);
		}
	}

	/// <summary>
	/// An extension class for creating a <see cref="CacheEntry{T}"/> from a value.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CacheEntryExtensions
	{
		/// <summary>
		/// Creates a <see cref="CacheEntry{T}"/> wrapping the given <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value to be wrapped.</param>
		/// <typeparam name="T">The type of value being wrapped.</typeparam>
		/// <returns>An instance of <see cref="CacheEntry{T}"/> containing the value.</returns>
		public static CacheEntry<T> ToCacheEntry<T>(this T value) => new CacheEntry<T>(value);
	}
}
