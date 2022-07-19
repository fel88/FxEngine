using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    /**
     * Loads the mesh data for a model from a collada XML file.
     * @author Karl
     *
     */
    public class GeometryLoader
    {

        //private static final Matrix4f CORRECTION = new Matrix4f().rotate((float) Math.toRadians(-90), new Vector3f(1, 0,0));
        private static Matrix4 CORRECTION = Matrix4.Identity;

        private XElement meshData;

        private List<VertexSkinData> vertexWeights;

        private float[] verticesArray;
        private float[] normalsArray;
        private float[] texturesArray;
        private int[] indicesArray;
        private int[] jointIdsArray;
        private float[] weightsArray;

        List<Vertex> vertices = new List<Vertex>();
        List<Vector2> textures = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        public GeometryLoader(XElement geometryNode, List<VertexSkinData> vertexWeights, ColladaParseContext ctx)
        {

            this.vertexWeights = vertexWeights;
            this.meshData = geometryNode.Element(XName.Get("geometry", ctx.Ns)).Element(XName.Get("mesh", ctx.Ns));
        }
        public ColladaParseContext Ctx;
        public MeshData extractModelData(ColladaParseContext ctx)
        {
            Ctx = ctx;
            readRawData();
            assembleVertices();
            removeUnusedVertices();
            initArrays();
            convertDataToArrays();
            convertIndicesListToArray();
            return new MeshData(verticesArray, texturesArray, normalsArray, indicesArray, jointIdsArray, weightsArray);
        }

        private void readRawData()
        {
            readPositions();
            readNormals();
            readTextureCoords();
        }

        private void readPositions()
        {
            String positionsId = meshData.Descendants(XName.Get("vertices", Ctx.Ns)).First().Element(XName.Get("input", Ctx.Ns)).Attribute("source").Value.Substring(1);
            XElement positionsData = meshData.getChildWithAttribute("source", "id", positionsId).Element(XName.Get("float_array", Ctx.Ns));
            int count = int.Parse(positionsData.Attribute("count").Value);
            String[] posData = positionsData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < count / 3; i++)
            {
                float x = float.Parse(posData[i * 3], CultureInfo.InvariantCulture);
                float y = float.Parse(posData[i * 3 + 1], CultureInfo.InvariantCulture);
                float z = float.Parse(posData[i * 3 + 2], CultureInfo.InvariantCulture);
                Vector4 position = new Vector4(x, y, z, 1);
                position = CORRECTION * position;
                //Matrix4.transform(CORRECTION, position, position);
                vertices.Add(new Vertex(vertices.Count(), new Vector3(position.X, position.Y, position.Z),
                    //vertexWeights[(vertices.Count)]
                    null
                    ));
            }
        }

        private void readNormals()
        {
            String normalsId = meshData.Descendants(XName.Get("polylist", Ctx.Ns)).First().getChildWithAttribute("input", "semantic", "NORMAL")
                    .Attribute("source").Value.Substring(1);
            XElement normalsData = meshData.getChildWithAttribute("source", "id", normalsId).Element(XName.Get("float_array", Ctx.Ns));
            int count = int.Parse(normalsData.Attribute("count").Value);
            String[] normData = normalsData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < count / 3; i++)
            {
                float x = float.Parse(normData[i * 3], CultureInfo.InvariantCulture);
                float y = float.Parse(normData[i * 3 + 1], CultureInfo.InvariantCulture);
                float z = float.Parse(normData[i * 3 + 2], CultureInfo.InvariantCulture);
                Vector4 norm = new Vector4(x, y, z, 0f);
                norm = CORRECTION * norm;
                //Matrix4.transform(CORRECTION, norm, norm);
                normals.Add(new Vector3(norm.X, norm.Y, norm.Z));
            }
        }

        private void readTextureCoords()
        {
            String texCoordsId = meshData.Descendants(XName.Get("polylist", Ctx.Ns)).First().getChildWithAttribute("input", "semantic", "TEXCOORD")
                    .Attribute("source").Value.Substring(1);
            XElement texCoordsData = meshData.getChildWithAttribute("source", "id", texCoordsId).Element(XName.Get("float_array", Ctx.Ns));
            int count = int.Parse(texCoordsData.Attribute("count").Value);
            String[] texData = texCoordsData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < count / 2; i++)
            {
                float s = float.Parse(texData[i * 2], CultureInfo.InvariantCulture);
                float t = float.Parse(texData[i * 2 + 1], CultureInfo.InvariantCulture);
                textures.Add(new Vector2(s, t));
            }
        }

        private void assembleVertices()
        {
            XElement poly = meshData.Element(XName.Get("polylist", Ctx.Ns));
            int typeCount = poly.Elements(XName.Get("input", Ctx.Ns)).Count();
            String[] indexData = poly.Descendants(XName.Get("p", Ctx.Ns)).First().Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < indexData.Length / typeCount; i++)
            {
                int positionIndex = int.Parse(indexData[i * typeCount]);
                int normalIndex = int.Parse(indexData[i * typeCount + 1]);
                int texCoordIndex = int.Parse(indexData[i * typeCount + 2]);
                processVertex(positionIndex, normalIndex, texCoordIndex);
            }
        }


        private Vertex processVertex(int posIndex, int normIndex, int texIndex)
        {
            Vertex currentVertex = vertices[posIndex];
            if (!currentVertex.isSet())
            {
                currentVertex.setTextureIndex(texIndex);
                currentVertex.setNormalIndex(normIndex);
                indices.Add(posIndex);
                return currentVertex;
            }
            else
            {
                return dealWithAlreadyProcessedVertex(currentVertex, texIndex, normIndex);
            }
        }

        private int[] convertIndicesListToArray()
        {
            this.indicesArray = new int[indices.Count()];
            for (int i = 0; i < indicesArray.Length; i++)
            {
                indicesArray[i] = indices[(i)];
            }
            return indicesArray;
        }

        private float convertDataToArrays()
        {
            float furthestPoint = 0;
            for (int i = 0; i < vertices.Count(); i++)
            {
                Vertex currentVertex = vertices[(i)];
                if (currentVertex.getLength() > furthestPoint)
                {
                    furthestPoint = currentVertex.getLength();
                }
                Vector3 position = currentVertex.getPosition();
                Vector2 textureCoord = textures[(currentVertex.getTextureIndex())];
                Vector3 normalVector = normals[(currentVertex.getNormalIndex())];
                verticesArray[i * 3] = position.X;
                verticesArray[i * 3 + 1] = position.Y;
                verticesArray[i * 3 + 2] = position.Z;
                texturesArray[i * 2] = textureCoord.X;
                texturesArray[i * 2 + 1] = 1 - textureCoord.Y;
                normalsArray[i * 3] = normalVector.X;
                normalsArray[i * 3 + 1] = normalVector.Y;
                normalsArray[i * 3 + 2] = normalVector.Z;
                VertexSkinData weights = currentVertex.getWeightsData();
                /*
                jointIdsArray[i * 3] = weights.jointIds[(0)];
                jointIdsArray[i * 3 + 1] = weights.jointIds[(1)];
                jointIdsArray[i * 3 + 2] = weights.jointIds[(2)];
                weightsArray[i * 3] = weights.weights[(0)];
                weightsArray[i * 3 + 1] = weights.weights[(1)];
                weightsArray[i * 3 + 2] = weights.weights[(2)];*/

            }
            return furthestPoint;
        }

        private Vertex dealWithAlreadyProcessedVertex(Vertex previousVertex, int newTextureIndex, int newNormalIndex)
        {
            if (previousVertex.hasSameTextureAndNormal(newTextureIndex, newNormalIndex))
            {
                indices.Add(previousVertex.getIndex());
                return previousVertex;
            }
            else
            {
                Vertex anotherVertex = previousVertex.getDuplicateVertex();
                if (anotherVertex != null)
                {
                    return dealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex);
                }
                else
                {
                    Vertex duplicateVertex = new Vertex(vertices.Count(), previousVertex.getPosition(), previousVertex.getWeightsData());
                    duplicateVertex.setTextureIndex(newTextureIndex);
                    duplicateVertex.setNormalIndex(newNormalIndex);
                    previousVertex.setDuplicateVertex(duplicateVertex);
                    vertices.Add(duplicateVertex);
                    indices.Add(duplicateVertex.getIndex());
                    return duplicateVertex;
                }

            }
        }

        private void initArrays()
        {
            this.verticesArray = new float[vertices.Count() * 3];
            this.texturesArray = new float[vertices.Count() * 2];
            this.normalsArray = new float[vertices.Count() * 3];
            this.jointIdsArray = new int[vertices.Count() * 3];
            this.weightsArray = new float[vertices.Count() * 3];
        }

        private void removeUnusedVertices()
        {
            foreach (Vertex vertex in vertices)
            {
                vertex.averageTangents();
                if (!vertex.isSet())
                {
                    vertex.setTextureIndex(0);
                    vertex.setNormalIndex(0);
                }
            }
        }

    }
}


