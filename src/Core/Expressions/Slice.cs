#region License
/* 
 * Copyright (C) 1999-2018 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// A slice is an improper subsequence of bits. Commonly used to isolate
	/// a byte register from a wider word- or dword-width register
	/// </summary>
	public class Slice : Expression
	{
		private byte offset;			// Bit-offset of value

		public Slice(DataType dt, Expression i, int bitOffset) : base(dt)
		{
			if (bitOffset > 255)
				throw new ArgumentOutOfRangeException("bitOffset", "Offset is too large.");
            if (bitOffset < 0)
                throw new ArgumentOutOfRangeException("bitOffset", "Offset must be non-negative.");
			Expression = i; offset = (byte) bitOffset;
		}

        public Expression Expression { get; set; }
        public int Offset { get { return offset; } }

        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitSlice(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitSlice(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitSlice(this);
		}

		public override Expression CloneExpression()
		{
			return new Slice(DataType, Expression, offset);
		}
	}
}
