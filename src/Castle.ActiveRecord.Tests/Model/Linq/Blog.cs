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
	using System.Collections;
	using Castle.ActiveRecord.Framework;

	[ActiveRecord("BlogTable")]
    public class Blog : ActiveRecordLinqBase<Blog>, IEquatable<Blog>
	{
        private int _id;
        private String _name;
        private String _author;
        private IList _posts;
        private IList _publishedposts;
        private IList _unpublishedposts;
        private IList _recentposts;

        public Blog()
        {
        }

        public Blog(int _id)
        {
            this._id = _id;
        }

        [PrimaryKey(PrimaryKeyType.Native)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Property]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Property]
        public String Author
        {
            get { return _author; }
            set { _author = value; }
        }

        [HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid")]
        public IList Posts
        {
            get { return _posts; }
            set { _posts = value; }
        }

        [HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid", Where = "published = 1")]
        public IList PublishedPosts
        {
            get { return _publishedposts; }
            set { _publishedposts = value; }
        }

        [HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid", Where = "published = 0")]
        public IList UnPublishedPosts
        {
            get { return _unpublishedposts; }
            set { _unpublishedposts = value; }
        }

        [HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid", OrderBy = "created desc")]
        public IList RecentPosts
        {
            get { return _recentposts; }
            set { _recentposts = value; }
        }

		public bool Equals(Blog other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._id == _id && Equals(other._name, _name) && Equals(other._author, _author);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Blog)) return false;
			return Equals((Blog) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = _id;
				result = (result*397) ^ (_name != null ? _name.GetHashCode() : 0);
				result = (result*397) ^ (_author != null ? _author.GetHashCode() : 0);
				result = (result*397) ^ (_posts != null ? _posts.GetHashCode() : 0);
				return result;
			}
		}

		public static bool operator ==(Blog left, Blog right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Blog left, Blog right)
		{
			return !Equals(left, right);
		}
    }
}
