// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace System.ServiceModel.Syndication
{
    // NOTE: This class implements Clone so if you add any members, please update the copy ctor
    public class SyndicationFeed : IExtensibleSyndicationObject
    {
        private static readonly HashSet<string> s_acceptedDays = new HashSet<string>(
            new string[] { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" },
            StringComparer.OrdinalIgnoreCase
        );        

        private Collection<SyndicationPerson> _authors;
        private Uri _baseUri;
        private Collection<SyndicationCategory> _categories;
        private Collection<SyndicationPerson> _contributors;
        private TextSyndicationContent _copyright;
        private TextSyndicationContent _description;
        private ExtensibleSyndicationObject _extensions = new ExtensibleSyndicationObject();
        private string _generator;
        private string _id;
        private Uri _imageUrl;
        private IEnumerable<SyndicationItem> _items;
        private string _language;
        private DateTimeOffset _lastUpdatedTime;
        private Collection<SyndicationLink> _links;
        private TextSyndicationContent _title;

        // optional RSS tags
        private SyndicationLink _documentation;
        private TimeSpan? _timeToLive;
        private Collection<int> _skipHours;
        private Collection<string> _skipDays;
        private SyndicationTextInput _textInput;

        public SyndicationFeed()
            : this((IEnumerable<SyndicationItem>)null)
        {
        }

        public SyndicationFeed(IEnumerable<SyndicationItem> items)
            : this(null, null, null, items)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink)
            : this(title, description, feedAlternateLink, null)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink, IEnumerable<SyndicationItem> items)
            : this(title, description, feedAlternateLink, null, DateTimeOffset.MinValue, items)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink, string id, DateTimeOffset lastUpdatedTime)
            : this(title, description, feedAlternateLink, id, lastUpdatedTime, null)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink, string id, DateTimeOffset lastUpdatedTime, IEnumerable<SyndicationItem> items)
        {
            if (title != null)
            {
                _title = new TextSyndicationContent(title);
            }
            if (description != null)
            {
                _description = new TextSyndicationContent(description);
            }
            if (feedAlternateLink != null)
            {
                this.Links.Add(SyndicationLink.CreateAlternateLink(feedAlternateLink));
            }
            _id = id;
            _lastUpdatedTime = lastUpdatedTime;
            _items = items;
        }

        protected SyndicationFeed(SyndicationFeed source, bool cloneItems)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _authors = FeedUtils.ClonePersons(source._authors);
            _categories = FeedUtils.CloneCategories(source._categories);
            _contributors = FeedUtils.ClonePersons(source._contributors);
            _copyright = FeedUtils.CloneTextContent(source._copyright);
            _description = FeedUtils.CloneTextContent(source._description);
            _extensions = source._extensions.Clone();
            _generator = source._generator;
            _id = source._id;
            _imageUrl = source._imageUrl;
            _language = source._language;
            _lastUpdatedTime = source._lastUpdatedTime;
            _links = FeedUtils.CloneLinks(source._links);
            _title = FeedUtils.CloneTextContent(source._title);
            _baseUri = source._baseUri;
            IList<SyndicationItem> srcList = source._items as IList<SyndicationItem>;
            if (srcList != null)
            {
                Collection<SyndicationItem> tmp = new NullNotAllowedCollection<SyndicationItem>();
                for (int i = 0; i < srcList.Count; ++i)
                {
                    tmp.Add((cloneItems) ? srcList[i].Clone() : srcList[i]);
                }
                _items = tmp;
            }
            else
            {
                if (cloneItems)
                {
                    throw new InvalidOperationException(SR.UnbufferedItemsCannotBeCloned);
                }

                _items = source._items;
            }
        }

        public Dictionary<XmlQualifiedName, string> AttributeExtensions
        {
            get { return _extensions.AttributeExtensions; }
        }

        public Collection<SyndicationPerson> Authors
        {
            get
            {
                if (_authors == null)
                {
                    _authors = new NullNotAllowedCollection<SyndicationPerson>();
                }
                return _authors;
            }
        }


        public Uri BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }

        public Collection<SyndicationCategory> Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new NullNotAllowedCollection<SyndicationCategory>();
                }
                return _categories;
            }
        }

        public Collection<SyndicationPerson> Contributors
        {
            get
            {
                if (_contributors == null)
                {
                    _contributors = new NullNotAllowedCollection<SyndicationPerson>();
                }
                return _contributors;
            }
        }

        public TextSyndicationContent Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

        public TextSyndicationContent Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public SyndicationElementExtensionCollection ElementExtensions
        {
            get { return _extensions.ElementExtensions; }
        }

        public string Generator
        {
            get { return _generator; }
            set { _generator = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Uri ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public IEnumerable<SyndicationItem> Items
        {
            get =>_items ?? (_items = new NullNotAllowedCollection<SyndicationItem>());
            set => _items = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        internal Exception LastUpdatedTimeException { get; set; }

        public DateTimeOffset LastUpdatedTime
        {
            get
            {
                if (LastUpdatedTimeException != null)
                {
                    throw LastUpdatedTimeException;
                }

                return _lastUpdatedTime;
            }
            set
            {
                LastUpdatedTimeException = null;
                _lastUpdatedTime = value;
            }
        }

        public Collection<SyndicationLink> Links
        {
            get
            {
                if (_links == null)
                {
                    _links = new NullNotAllowedCollection<SyndicationLink>();
                }
                return _links;
            }
        }

        public TextSyndicationContent Title
        {
            get { return _title; }
            set { _title = value; }
        }

        internal SyndicationLink InternalDocumentation => _documentation;

        public SyndicationLink Documentation
        {
            get
            {
                if (_documentation == null)
                {
                    _documentation = TryReadDocumentationFromExtension(ElementExtensions);
                }

                return _documentation;
            }
            set
            {
                _documentation = value;
            }
        }

        internal TimeSpan? InternalTimeToLive => _timeToLive;

        public TimeSpan? TimeToLive
        {
            get
            {
                if (!_timeToLive.HasValue)
                {
                    _timeToLive = TryReadTimeToLiveFromExtension(ElementExtensions);
                }

                return _timeToLive;
            }
            set
            {
                if (value.HasValue && (value.Value.Milliseconds != 0 || value.Value.Seconds != 0 || value.Value.TotalMinutes < 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Value, SR.InvalidTimeToLiveValue);
                }

                _timeToLive = value;
            }
        }

        internal Collection<int> InternalSkipHours => _skipHours;

        public Collection<int> SkipHours
        {
            get
            {
                if (_skipHours == null)
                {
                    var skipHours = new Collection<int>();
                    TryReadSkipHoursFromExtension(ElementExtensions, skipHours);
                    _skipHours = skipHours;
                }

                return _skipHours;
            }
        }

        internal Collection<string> InternalSkipDays => _skipDays;

        public Collection<string> SkipDays
        {
            get
            {
                if (_skipDays == null)
                {
                    var skipDays = new Collection<string>();
                    TryReadSkipDaysFromExtension(ElementExtensions, skipDays);
                    _skipDays = skipDays;
                }

                return _skipDays;
            }
        }

        internal SyndicationTextInput InternalTextInput => _textInput;

        public SyndicationTextInput TextInput
        {
            get
            {
                if (_textInput == null)
                {
                    _textInput = TryReadTextInputFromExtension(ElementExtensions);
                }

                return _textInput;
            }
            set
            {
                _textInput = value;
            }
        }

        private SyndicationLink TryReadDocumentationFromExtension(SyndicationElementExtensionCollection elementExtensions)
        {
            SyndicationElementExtension documentationElement = elementExtensions
                                      .Where(e => e.OuterName == Rss20Constants.DocumentationTag && e.OuterNamespace == Rss20Constants.Rss20Namespace)
                                      .FirstOrDefault();

            if (documentationElement == null)
                return null;

            using (XmlReader reader = documentationElement.GetReader())
            {
                SyndicationLink documentation = Rss20FeedFormatter.ReadAlternateLink(reader, BaseUri, SyndicationFeedFormatter.DefaultUriParser, preserveAttributeExtensions: true);
                return documentation;
            }
        }

        private TimeSpan? TryReadTimeToLiveFromExtension(SyndicationElementExtensionCollection elementExtensions)
        {
            SyndicationElementExtension timeToLiveElement = elementExtensions
                                      .FirstOrDefault(e => e.OuterName == Rss20Constants.TimeToLiveTag && e.OuterNamespace == Rss20Constants.Rss20Namespace);

            if (timeToLiveElement == null)
                return null;

            using (XmlReader reader = timeToLiveElement.GetReader())
            {
                string value = reader.ReadElementString();
                if (int.TryParse(value, out int timeToLive))
                {
                    if (timeToLive >= 0)
                    {
                        return TimeSpan.FromMinutes(timeToLive);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        private void TryReadSkipHoursFromExtension(SyndicationElementExtensionCollection elementExtensions, Collection<int> skipHours)
        {
            SyndicationElementExtension skipHoursElement = elementExtensions
                                      .Where(e => e.OuterName == Rss20Constants.SkipHoursTag && e.OuterNamespace == Rss20Constants.Rss20Namespace)
                                      .FirstOrDefault();

            if (skipHoursElement == null)
                return;

            using (XmlReader reader = skipHoursElement.GetReader())
            {
                reader.ReadStartElement();

                while (reader.IsStartElement())
                {
                    if (reader.LocalName == Rss20Constants.HourTag)
                    {
                        string value = reader.ReadElementString();
                        int hour;
                        bool parsed = int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out hour);

                        if (!parsed || (hour < 0 || hour > 23))
                        {
                            throw new FormatException(string.Format(SR.InvalidSkipHourValue, value));
                        }

                        skipHours.Add(hour);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }
        }

        private void TryReadSkipDaysFromExtension(SyndicationElementExtensionCollection elementExtensions, Collection<string> skipDays)
        {
            SyndicationElementExtension skipDaysElement = elementExtensions
                                      .Where(e => e.OuterName == Rss20Constants.SkipDaysTag && e.OuterNamespace == Rss20Constants.Rss20Namespace)
                                      .FirstOrDefault();

            if (skipDaysElement == null)
                return;

            using (XmlReader reader = skipDaysElement.GetReader())
            {
                reader.ReadStartElement();

                while (reader.IsStartElement())
                {
                    if (reader.LocalName == Rss20Constants.DayTag)
                    {
                        string day = reader.ReadElementString();

                        //Check if the day is actually an accepted day.
                        if (IsValidDay(day))
                        {
                            skipDays.Add(day);
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }
                }

                reader.ReadEndElement();
            }
        }

        private static bool IsValidDay(string day) => s_acceptedDays.Contains(day);

        private SyndicationTextInput TryReadTextInputFromExtension(SyndicationElementExtensionCollection elementExtensions)
        {
            SyndicationElementExtension textInputElement = elementExtensions
                                      .Where(e => e.OuterName == Rss20Constants.TextInputTag && e.OuterNamespace == Rss20Constants.Rss20Namespace)
                                      .FirstOrDefault();

            if (textInputElement == null)
                return null;

            var textInput = new SyndicationTextInput();
            using (XmlReader reader = textInputElement.GetReader())
            {
                reader.ReadStartElement();
                while (reader.IsStartElement())
                {
                    string name = reader.LocalName;
                    string value = reader.ReadElementString();

                    switch (name)
                    {
                        case Rss20Constants.DescriptionTag:
                            textInput.Description = value;
                            break;

                        case Rss20Constants.TitleTag:
                            textInput.Title = value;
                            break;

                        case Rss20Constants.LinkTag:
                            textInput.Link = new SyndicationLink(new Uri(value, UriKind.RelativeOrAbsolute));
                            break;

                        case Rss20Constants.NameTag:
                            textInput.Name = value;
                            break;

                        default:
                            break;
                    }
                }

                reader.ReadEndElement();
            }

            return IsValidTextInput(textInput) ? textInput : null;
        }

        private static bool IsValidTextInput(SyndicationTextInput textInput)
        {
            //All textInput items are required, we check if all items were instantiated.
            return textInput.Description != null && textInput.Title != null && textInput.Name != null && textInput.Link != null;
        }

        public static SyndicationFeed Load(XmlReader reader)
        {
            return Load<SyndicationFeed>(reader);
        }

        public static TSyndicationFeed Load<TSyndicationFeed>(XmlReader reader)
            where TSyndicationFeed : SyndicationFeed, new()
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            Atom10FeedFormatter<TSyndicationFeed> atomSerializer = new Atom10FeedFormatter<TSyndicationFeed>();
            if (atomSerializer.CanRead(reader))
            {
                atomSerializer.ReadFrom(reader);
                return atomSerializer.Feed as TSyndicationFeed;
            }
            Rss20FeedFormatter<TSyndicationFeed> rssSerializer = new Rss20FeedFormatter<TSyndicationFeed>();
            if (rssSerializer.CanRead(reader))
            {
                rssSerializer.ReadFrom(reader);
                return rssSerializer.Feed as TSyndicationFeed;
            }

            throw new XmlException(SR.Format(SR.UnknownFeedXml, reader.LocalName, reader.NamespaceURI));
        }

        public virtual SyndicationFeed Clone(bool cloneItems)
        {
            return new SyndicationFeed(this, cloneItems);
        }

        public Atom10FeedFormatter GetAtom10Formatter()
        {
            return new Atom10FeedFormatter(this);
        }

        public Rss20FeedFormatter GetRss20Formatter()
        {
            return GetRss20Formatter(true);
        }

        public Rss20FeedFormatter GetRss20Formatter(bool serializeExtensionsAsAtom)
        {
            return new Rss20FeedFormatter(this, serializeExtensionsAsAtom);
        }

        public void SaveAsAtom10(XmlWriter writer)
        {
            this.GetAtom10Formatter().WriteTo(writer);
        }

        public void SaveAsRss20(XmlWriter writer)
        {
            this.GetRss20Formatter().WriteTo(writer);
        }

        protected internal virtual SyndicationCategory CreateCategory()
        {
            return new SyndicationCategory();
        }

        protected internal virtual SyndicationItem CreateItem()
        {
            return new SyndicationItem();
        }

        protected internal virtual SyndicationLink CreateLink()
        {
            return new SyndicationLink();
        }

        protected internal virtual SyndicationPerson CreatePerson()
        {
            return new SyndicationPerson();
        }

        protected internal virtual bool TryParseAttribute(string name, string ns, string value, string version)
        {
            return false;
        }

        protected internal virtual bool TryParseElement(XmlReader reader, string version)
        {
            return false;
        }

        protected internal virtual void WriteAttributeExtensions(XmlWriter writer, string version)
        {
            _extensions.WriteAttributeExtensions(writer);
        }

        protected internal virtual void WriteElementExtensions(XmlWriter writer, string version)
        {
            _extensions.WriteElementExtensions(writer, ShouldSkipWritingElements);
        }

        private bool ShouldSkipWritingElements(string localName, string ns)
        {
            if (ns == Rss20Constants.Rss20Namespace)
            {
                switch (localName)
                {
                    case Rss20Constants.DocumentationTag:
                        return InternalDocumentation != null;

                    case Rss20Constants.TimeToLiveTag:
                        return InternalTimeToLive != null;

                    case Rss20Constants.TextInputTag:
                        return InternalTextInput != null;

                    case Rss20Constants.SkipHoursTag:
                        return InternalSkipHours != null;

                    case Rss20Constants.SkipDaysTag:
                        return InternalSkipDays != null;
                }
            }

            return false;
        }

        internal void LoadElementExtensions(XmlReader readerOverUnparsedExtensions, int maxExtensionSize)
        {
            _extensions.LoadElementExtensions(readerOverUnparsedExtensions, maxExtensionSize);
        }

        internal void LoadElementExtensions(XmlBuffer buffer)
        {
            _extensions.LoadElementExtensions(buffer);
        }
    }
}
