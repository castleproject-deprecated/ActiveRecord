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

namespace Castle.ActiveRecord.Tests.Model.Linq
{
	using System;

	[ActiveRecord]
    public class Widget : ActiveRecordBase, IEquatable<Widget>
    {
        [PrimaryKey]
        public int Id
        {
            get;
            set;
        }

        [Property]
        public string Name
        {
            get;
            set;
        }

        public static void DeleteAll()
        {
            ActiveRecordMediator.DeleteAll(typeof (Widget));
        }

    	public bool Equals(Widget other)
    	{
    		if (ReferenceEquals(null, other)) return false;
    		if (ReferenceEquals(this, other)) return true;
    		return other.Id == Id && Equals(other.Name, Name);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(null, obj)) return false;
    		if (ReferenceEquals(this, obj)) return true;
    		if (obj.GetType() != typeof (Widget)) return false;
    		return Equals((Widget) obj);
    	}

    	public override int GetHashCode()
    	{
    		unchecked
    		{
    			return (Id*397) ^ (Name != null ? Name.GetHashCode() : 0);
    		}
    	}
    }
}
