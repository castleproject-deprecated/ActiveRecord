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

using System;

namespace Castle.ActiveRecord.Framework.Config
{
	/// <summary>
	/// Enables the fluent configuration of ActiveRecord.
	/// </summary>
	public static class Configure
	{
		/// <summary>
		/// Builds a fluent configuration for general ActiveRecord settings.
		/// </summary>
		public static FluentActiveRecordConfiguration ActiveRecord
		{
			get { return new FluentActiveRecordConfiguration(); }
		}

		/// <summary>
		/// Builds an ActiveRecord storage specifiaction fluently.
		/// </summary>
		public static FluentStorageConfiguration Storage	
		{
			get { return new FluentStorageConfiguration(); }
		}
	}
}