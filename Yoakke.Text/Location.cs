﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Text
{
    /// <summary>
    /// Represents a range in some source file.
    /// </summary>
    public readonly struct Location
    {
        /// <summary>
        /// The source file.
        /// </summary>
        public readonly ISourceFile File;
        /// <summary>
        /// The range.
        /// </summary>
        public readonly Range Range;

        /// <summary>
        /// Initializes a new <see cref="Location"/>.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="range">The range in the file.</param>
        public Location(ISourceFile file, Range range)
        {
            File = file;
            Range = range;
        }
    }
}
