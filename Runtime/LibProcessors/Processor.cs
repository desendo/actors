//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games

using System;

namespace Pixeye.Framework
{
	public abstract class Processor : IDisposable
	{
		protected Processor()
		{
			ProcessorGroups.Setup(this);
			ProcessorSignals.Add(this);
			Toolbox.disposables.Add(this);
		}

		public void Dispose()
		{
			ProcessorSignals.Remove(this);
			ProcessorUpdate.Remove(this);

			OnDispose();
		}

		protected virtual void OnDispose()
		{
		}
	}

	// public static class ProcessorHelpers
	// {
	// 	public static T Set<T>(this Processor proc, Op op) where T : GroupEvents, new()
	// 	{
	// 		var evs = new T();
	//
	//
	// 		// if ((op & Op.Add) == Op.Add)
	// 		// {
	// 		//
	// 		//
	// 		// 	if (eventsAddLength >= eventsAdd.Length)
	// 		// 	{
	// 		// 		Array.Resize(ref eventsAdd, eventsAddLength + 1);
	// 		// 	}
	// 		//
	// 		// 	eventsAdd[eventsAddLength++] = evs;
	// 		// }
	// 		//
	// 		//
	// 		// if ((op & Op.Remove) == Op.Remove)
	// 		// {
	// 		// 	if (eventsRemoveLength >= eventsRemove.Length)
	// 		// 	{
	// 		// 		Array.Resize(ref eventsRemove, eventsRemoveLength + 1);
	// 		// 	}
	// 		//
	// 		// 	eventsRemove[eventsRemoveLength++] = evs;
	// 		// }
	//
	// 		return evs;
	// 	}
	// }

	public abstract class ProcessorGroup : GroupEvents, IDisposable
	{
		protected ProcessorGroup()
		{
			ProcessorInitializer.Setup(this);
			ProcessorSignals.Add(this);
			Toolbox.disposables.Add(this);
		}

		public void Dispose()
		{
			ProcessorSignals.Remove(this);
			ProcessorUpdate.Remove(this);

			OnDispose();
		}
		protected virtual void OnDispose()
		{
		}
	}

	public abstract class Processor<T> : ProcessorGroup
	{
		protected Group<T> source = null;
	}

	public abstract class Processor<T, Y> : ProcessorGroup
	{
		protected Group<T, Y> source = null;
	}

	public abstract class Processor<T, Y, U> : ProcessorGroup
	{
		protected Group<T, Y, U> source = null;
	}

	public abstract class Processor<T, Y, U, I> : ProcessorGroup
	{
		protected Group<T, Y, U, I> source = null;
	}

	public abstract class Processor<T, Y, U, I, O> : ProcessorGroup
	{
		protected Group<T, Y, U, I, O> source = null;
	}

	public abstract class Processor<T, Y, U, I, O, P> : ProcessorGroup
	{
		protected Group<T, Y, U, I, O, P> source = null;
	}

	public abstract class Processor<T, Y, U, I, O, P, A> : ProcessorGroup
	{
		protected Group<T, Y, U, I, O, P, A> source = null;
	}

	public abstract class Processor<T, Y, U, I, O, P, A, S> : ProcessorGroup
	{
		protected Group<T, Y, U, I, O, P, A, S> source = null;
	}

	public abstract class Processor<T, Y, U, I, O, P, A, S, D> : ProcessorGroup
	{
		protected Group<T, Y, U, I, O, P, A, S, D> source = null;
	}

	public abstract class Processor<T, Y, U, I, O, P, A, S, D, F> : ProcessorGroup
	{
		protected Group<T, Y, U, I, O, P, A, S, D, F> source = null;
	}
}