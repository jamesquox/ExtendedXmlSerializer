// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nag�rski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core
{
	class ConditionalCommand<T> : ICommand<T>
	{
		readonly ISpecification<T> _specification;
		readonly ICommand<T> _true;
		readonly ICommand<T> _false;

		public ConditionalCommand(ISpecification<T> specification, ICommand<T> @true, ICommand<T> @false)
		{
			_specification = specification;
			_true = @true;
			_false = @false;
		}

		public void Execute(T parameter)
		{
			var command = _specification.IsSatisfiedBy(parameter) ? _true : _false;
			command.Execute(parameter);
		}
	}

	class SpecificationCommand<T> : ICommand<T>
	{
		readonly ISpecification<T> _specification;
		readonly ICommand<T> _command;

		public SpecificationCommand(ICommand<T> command) : this(new ConditionalSpecification<T>(), command) {}

		public SpecificationCommand(ISpecification<T> specification, ICommand<T> command)
		{
			_specification = specification;
			_command = command;
		}

		public void Execute(T parameter)
		{
			if (_specification.IsSatisfiedBy(parameter))
			{
				_command.Execute(parameter);
			}
		}
	}
}