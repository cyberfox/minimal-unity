using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DAZ
{
    public class DataCollection<T>
    {
        public int count { get; set; }
        public List<List<T>> values { get; set; }
    }

    public class SimpleCollection<T>
    {
        public int count { get; set; }
        public List<T> values { get; set; }
    }

    public class Vertices : DataCollection<float> { }

    public class PolygonGroups : SimpleCollection<string> { }

    public class PolyList : DataCollection<int> { }

    public class Region
    {
        public string id  { get; set; }
        public string label { get; set; }
        public string display_hint { get; set; }
        public List<Region> children { get; set; }
        public SimpleCollection<int> map { get; set; }
    }

    public class Geometry
    {
        public string id  { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string edge_interpolation_mode { get; set; }
        public string subd_mormal_smoothing_mode { get; set; }

        public Vertices vertices { get; set; }
        public PolygonGroups polygon_groups { get; set; }
        public PolygonGroups polygon_material_groups { get; set; }
        public PolyList polylist { get; set; }
        public string default_uv_set { get; set; }
        public Region root_region { get; set; }
        // public {something} graft { get; set; }
        // public {something} extra { get; set; }
    }

    public class Point
    {
        public string id  { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public bool visible  { get; set; }
        public bool auto_follow  { get; set; }
        public float value { get; set; }
        public long min { get; set; }
        public long max { get; set; }
        public bool display_as_percent { get; set; }
        public float step_size { get; set; }
    }

    public class Node
    {
        public string id  { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string rotation_order { get; set; }
        public bool inherits_scale { get; set; }
        public List<Point> center_point { get; set; }
        public List<Point> end_point { get; set; }
        public List<Point> orientation { get; set; }
        public List<Point> rotation { get; set; }
        public List<Point> translation { get; set; }
        public List<Point> scale { get; set; }
        public Point general_scale { get; set; }
        public Presentation presentation { get; set; }
        // public {something} extra { get; set; }
    }

    public class Presentation
    {
        public string type { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string icon_large { get; set; }
        public List<List<float>> colors { get; set; }
        public string preferred_base { get; set; }
    }

    public class Contributor
    {
        public string author { get; set; }
        public string email { get; set; }
        public string website { get; set; }
    }

    public class AssetInfo
    {
       public string id { get; set; }
       public string type { get; set; }
       public string revision { get; set; }
       public string modified { get; set; }
       public Contributor contributor { get; set; }
    }

    public class UVSet
    {
        public string id  { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public int vertex_count { get; set; }
        public DataCollection<float> uvs { get; set; }
        public List<List<int>> polygon_vertex_indices { get; set; }
    }

    public class DSF
    {
        public string file_version { get; set; }
        public AssetInfo asset_info { get; set; }
        public List<Geometry> geometry_library { get; set; }
        public List<UVSet> uv_set_library { get; set; }
        public List<Node> node_library { get; set; }
        // public List<JObject> modifier_library { get; set; }
    }

    public class ImageMap
    {
        public string url { get; set; }
        public string label { get; set; }
        public List<float> color { get; set; }
        public float transparency { get; set; }
        public bool invert { get; set; }
        public float rotation { get; set; }
        public bool xmirror { get; set; }
        public bool ymirror { get; set; }
        public float xscale { get; set; }
        public float yscale { get; set; }
        public float xoffset { get; set; }
        public float yoffset { get; set; }
        public string operation { get; set; }
    }

    public class Image
    {
        public string id  { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public float map_gamma { get; set; }
        public JObject map_size { get; set; }
        public ImageMap map { get; set; }
    }

    public class DUF
    {
        public AssetInfo asset_info { get; set; }
        public List<Image> image_library { get; set; }
        public List<Material> material_library { get; set; }
    }

    public class Utility
    {
        private const ushort GZIP_LEAD_BYTES = 0x8b1f;

        public static StreamReader OpenPossiblyCompressedFile(string filename)
        {
            FileStream inputStream = new FileStream(filename, FileMode.Open);
            byte[] bytes = new byte[4];
            inputStream.Read(bytes, 0, 4);
            inputStream.Seek(0, SeekOrigin.Begin);
            StreamReader sr;
            if (BitConverter.ToUInt16(bytes, 0) == GZIP_LEAD_BYTES)
            {
                GZipStream inStream = new GZipStream(inputStream, CompressionMode.Decompress);
                sr = new StreamReader(inStream);
            }
            else
            {
                sr = new StreamReader(inputStream);
            }

            return sr;
        }

        public static DSF LoadDSF(string filename)
        {
            StreamReader sr = OpenPossiblyCompressedFile(filename);
            var js = JsonSerializer.CreateDefault();
            DSF dsf = (DSF)js.Deserialize(sr, typeof(DSF));

            return dsf;
        }
    }
}
