// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Base class that allows specifying an alternate table for an object's field or property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false), Serializable]
	public abstract class WithAccessOptionalTableAttribute : WithAccessAttribute
	{
		private String table;
	    private bool optimisticLock = true;

		/// <summary>
		/// Gets or sets the table name if joined
		/// </summary>
		public String Table
		{
			get { return table; }
			set { table = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating if changes to this member will increment the containing entity's version.
        /// </summary>
        public bool OptimisticLock
        {
            get { return optimisticLock; }
            set { optimisticLock = value; }
        }
    }
}
