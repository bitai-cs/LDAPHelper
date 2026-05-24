using System.Text;
using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

/// <summary>
/// Mock implementation of ILdapAttributeAdapter that mimics Novell.Directory.Ldap.LdapAttribute behavior
/// </summary>
public class MockLdapAttributeAdapter : ILdapAttributeAdapter
{
    private readonly string _baseName;
    private readonly string[] _subTypes;
    private List<byte[]> _values;

    #region Constructors

    /// <summary>
    /// Constructs an attribute with no values
    /// </summary>
    public MockLdapAttributeAdapter(string attributeName) {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException("Attribute name cannot be null or empty");

        Name = attributeName;
        _baseName = GetBaseName(attributeName);
        _subTypes = GetSubtypes(attributeName);
        _values = new List<byte[]>();
    }

    ///// <summary>
    ///// Constructs an attribute with a single value (auto-detects type)
    ///// </summary>
    //public MockLdapAttributeAdapter(object value) : this("unknown", value) {
    //}

    /// <summary>
    /// Constructs an attribute with a named value (auto-detects type)
    /// </summary>
    public MockLdapAttributeAdapter(string attributeName, object value) : this(attributeName) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");

        AddValue(value);
    }

    /// <summary>
    /// Constructs an attribute with a byte array value
    /// </summary>
    public MockLdapAttributeAdapter(string attributeName, byte[] value) : this(attributeName) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");

        AddValue(value);
    }

    /// <summary>
    /// Constructs an attribute with a string value
    /// </summary>
    public MockLdapAttributeAdapter(string attributeName, string value) : this(attributeName) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");

        AddValue(value);
    }

    /// <summary>
    /// Constructs an attribute with multiple string values
    /// </summary>
    public MockLdapAttributeAdapter(string attributeName, string[] values) : this(attributeName) {
        if (values == null)
            throw new ArgumentException("Attribute values array cannot be null");

        foreach (var value in values) {
            if (value == null)
                throw new ArgumentException("Attribute value in array cannot be null");
            AddValue(value);
        }
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public MockLdapAttributeAdapter(MockLdapAttributeAdapter original) {
        if (original == null)
            throw new ArgumentException("Original attribute cannot be null");

        Name = original.Name;
        _baseName = original._baseName;
        _subTypes = original._subTypes != null ? (string[])original._subTypes.Clone() : null;
        _values = original._values != null
            ? original._values.Select(v => (byte[])v.Clone()).ToList()
            : new List<byte[]>();
    }

    #endregion

    #region Properties

    public string Name { get; }

    public byte[] ByteValue => _values.Count > 0 ? (byte[])_values[0].Clone() : null;

    public string StringValue => _values.Count > 0 ? Encoding.UTF8.GetString(_values[0]) : null;

    public string[] StringValueArray => _values.Count > 0
        ? _values.Select(v => Encoding.UTF8.GetString(v)).ToArray()
        : Array.Empty<string>();

    /// <summary>
    /// Returns all values as byte arrays
    /// </summary>
    public byte[][] ByteValueArray => _values.Count > 0
        ? _values.Select(v => (byte[])v.Clone()).ToArray()
        : Array.Empty<byte[]>();

    /// <summary>
    /// Returns the language subtype of the attribute (e.g., "lang-ja")
    /// </summary>
    public string LangSubtype {
        get {
            if (_subTypes != null) {
                foreach (var subtype in _subTypes) {
                    if (subtype.StartsWith("lang-"))
                        return subtype;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Number of values in the attribute
    /// </summary>
    public int ValueCount => _values.Count;

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a string value to the attribute
    /// </summary>
    public void AddValue(string value) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");
        AddValue(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Adds a byte array value to the attribute
    /// </summary>
    public void AddValue(byte[] value) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");

        // Check for duplicate
        var clonedValue = (byte[])value.Clone();
        if (!_values.Any(v => v.SequenceEqual(clonedValue))) {
            _values.Add(clonedValue);
        }
    }

    public void AddValue(object value) {
        if (value is byte[] bytes)
            AddValue(bytes);
        else if (value is string[] stringArray) {
            foreach (var str in stringArray)
                AddValue(str);
        }
        else if (value is string str)
            AddValue(str);
        else
            AddValue(value?.ToString() ?? throw new ArgumentException("Value cannot be null"));
    }

    /// <summary>
    /// Adds a base64 encoded value (decodes and stores as bytes)
    /// </summary>
    public void AddBase64Value(string base64String) {
        if (base64String == null)
            throw new ArgumentException("Base64 value cannot be null");
        AddValue(Convert.FromBase64String(base64String));
    }
    
    /// <summary>
    /// Removes a string value from the attribute
    /// </summary>
    public void RemoveValue(string value) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");
        RemoveValue(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Removes a byte array value from the attribute
    /// </summary>
    public void RemoveValue(byte[] value) {
        if (value == null)
            throw new ArgumentException("Attribute value cannot be null");

        var index = _values.FindIndex(v => v.SequenceEqual(value));
        if (index >= 0) {
            _values.RemoveAt(index);
        }
    }

    /// <summary>
    /// Removes all values from the attribute
    /// </summary>
    public void ClearValues() {
        _values.Clear();
    }

    /// <summary>
    /// Checks if the attribute has a specific value
    /// </summary>
    public bool HasValue(string value) {
        if (value == null) return false;
        return HasValue(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Checks if the attribute has a specific byte value
    /// </summary>
    public bool HasValue(byte[] value) {
        if (value == null) return false;
        return _values.Any(v => v.SequenceEqual(value));
    }

    /// <summary>
    /// Returns the base name of the attribute (e.g., "cn" from "cn;lang-ja;phonetic")
    /// </summary>
    public string GetBaseName() {
        return _baseName;
    }

    /// <summary>
    /// Returns all subtypes of the attribute
    /// </summary>
    public string[] GetSubtypes() {
        return _subTypes != null ? (string[])_subTypes.Clone() : null;
    }

    /// <summary>
    /// Checks if the attribute has a specific subtype
    /// </summary>
    public bool HasSubtype(string subtype) {
        if (subtype == null)
            throw new ArgumentException("Subtype cannot be null");

        if (_subTypes != null) {
            return _subTypes.Any(s => s.Equals(subtype, StringComparison.OrdinalIgnoreCase));
        }
        return false;
    }

    /// <summary>
    /// Creates a deep clone of this attribute
    /// </summary>
    public MockLdapAttributeAdapter Clone() {
        return new MockLdapAttributeAdapter(this);
    }

    /// <summary>
    /// Compares this attribute with another by name
    /// </summary>
    public int CompareTo(MockLdapAttributeAdapter other) {
        if (other == null) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    #endregion

    #region Private Methods
   
    private static string GetBaseName(string attributeName) {
        if (string.IsNullOrEmpty(attributeName))
            return attributeName;

        var semiColonIndex = attributeName.IndexOf(';');
        return semiColonIndex == -1 ? attributeName : attributeName.Substring(0, semiColonIndex);
    }

    private static string[] GetSubtypes(string attributeName) {
        if (string.IsNullOrEmpty(attributeName))
            return null;

        var semiColonIndex = attributeName.IndexOf(';');
        if (semiColonIndex == -1)
            return null;

        var subtypePart = attributeName.Substring(semiColonIndex + 1);
        return subtypePart.Split(';');
    }

    #endregion

    #region Overrides

    public override string ToString() {
        var sb = new StringBuilder("MockLdapAttribute: ");
        sb.Append("{type='").Append(Name).Append('\'');

        if (_values.Count > 0) {
            sb.Append(", ");
            sb.Append(_values.Count == 1 ? "value='" : "values='");

            for (int i = 0; i < _values.Count; i++) {
                if (i != 0)
                    sb.Append("','");

                var value = _values[i];
                if (value.Length > 0) {
                    var text = Encoding.UTF8.GetString(value);
                    if (string.IsNullOrEmpty(text))
                        sb.Append("<binary value, length:").Append(value.Length).Append('>');
                    else
                        sb.Append(text);
                }
            }
            sb.Append('\'');
        }

        sb.Append('}');
        return sb.ToString();
    }

    #endregion
}