﻿using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaLoader
    {
        public static AnimationData loadColladaAnimation(string colladaFile, ColladaParseContext ctx)
        {
            var doc = ctx.DataProvider.LoadXml(colladaFile);
            XElement node = doc.Elements().First();
            XElement animNode = node.Element(XName.Get("library_animations", ctx.Ns));
            XElement jointsNode = node.Element(XName.Get("library_visual_scenes", ctx.Ns));
            ColladaAnimationLoader loader = new ColladaAnimationLoader(animNode, jointsNode);
            AnimationData animData = loader.extractAnimation(ctx);
            return animData;
        }

    }
}


