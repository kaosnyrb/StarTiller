using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KNPE
{
    public class Model3d
    {
        public Model ContentModel;
        private ModelBone [] ContentModelBones;
        Matrix[] boneTransforms;
        private int NumberOfBones;
        public BoundingSphere Bounding = new BoundingSphere();
        public bool Fog = true;
        public float Scale = 1.0f;
        public Effect Shader;
        public Effect Phong;
        public Texture2D tex;

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            ContentModel = theContentManager.Load<Model>(theAssetName);
            NumberOfBones = ContentModel.Bones.Count;
            if (NumberOfBones > 1)//no point for single bone models.
            {
                ContentModelBones = new ModelBone[NumberOfBones];
                for (int i = 0; i < NumberOfBones; i++)
                {
                    ContentModelBones[i] = ContentModel.Bones[i];
                }
                boneTransforms = new Matrix[NumberOfBones];
            }
            if (ContentModel.Bones.Count > 1)
            {
                if (boneTransforms != null)
                {
                    ContentModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
                }
                for (int i = 0; i < NumberOfBones; i++)
                {
                    boneTransforms[i].Translation = boneTransforms[i].Translation;// *scale;
                }
            }
            InitShader(theContentManager);

        }
        public void InitShader(ContentManager theContentManager)
        {
            Shader = theContentManager.Load<Effect>("Shader");
            Phong = theContentManager.Load<Effect>("Shaders/Phong");
        }
        public int ResetEffectSettings = 0;

        public void Draw(Matrix world, Matrix view, Matrix projection, float scale, float Alpha)
        {
            if (ContentModel.Bones.Count > 1)
            {
                ContentModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
                for (int i = 0; i < NumberOfBones; i++)
                {
                    boneTransforms[i].Translation = boneTransforms[i].Translation;// *scale;
                }
            }
            // Draw the model.
            foreach (ModelMesh mesh in ContentModel.Meshes)
            {
                mesh.Draw();

                foreach (Effect effect in mesh.Effects)
                {
                    BasicEffect basicEffect = effect as BasicEffect;

                    if (basicEffect != null)
                    {
                        if (boneTransforms != null)
                        {
                            boneTransforms[mesh.ParentBone.Index].Translation = Vector3.Zero;
                            basicEffect.World = world * boneTransforms[mesh.ParentBone.Index];  //* Matrix.CreateScale(scale) //scale messes with the entitymanager
                        }
                        else
                        {
                            basicEffect.World = world;
                        }
                        basicEffect.View = view;
                        basicEffect.Projection = projection;
                    }
                    else
                    {
                        // Set parameters for a billboard effect.
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["LightDirection"].SetValue(Graphics_Core.LightDirection);
                    }
                }
                mesh.Draw();
            }
        }
        public void DrawWithCull(Matrix world, Matrix view, Matrix projection, float scale, float Alpha)
        {
            // Draw the model.
            foreach (ModelMesh mesh in ContentModel.Meshes)
            {
                if (!Camera.Frustum.Intersects(mesh.BoundingSphere))
                {
                    continue;
                }
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (Bounding.Radius == 0)
                    {
                        Bounding = mesh.BoundingSphere;
                    }
                    if (boneTransforms != null)
                    {
                        effect.World = world * boneTransforms[mesh.ParentBone.Index];
                    }
                    else
                    {
                        effect.World = Matrix.CreateScale(Scale) * world;
                    }

                    effect.View = view;
                    effect.Projection = projection;

                }
                mesh.Draw();
            }
        }
        
    }
    class RenderEntity3d
    {
        public Vector3 Position = new Vector3(0, 0, 0);
        public Matrix Rotation = new Matrix();
        public float Scale = -1;
        public float Alpha = 1;
        public int ModelID = 0;
        public bool Active = false;
    }
    public static class RenderEntity3d_Manager
    {
        public static int MAXMODELS = 20;
        public static int MAXRENDERENTITYS = 300;
        private static RenderEntity3d[] RenderEntity3dList;
        public static Model3d[] ModelList;

        static RenderEntity3d_Manager()
        {
            ModelList = new Model3d[MAXMODELS];
            for (int i = 0; i < MAXMODELS; i++)
            {
                ModelList[i] = new Model3d();
            }
            RenderEntity3dList = new RenderEntity3d[MAXRENDERENTITYS];
            for (int i = 0; i < MAXRENDERENTITYS; i++)
            {
                RenderEntity3dList[i] = new RenderEntity3d();
            }
        }
        public static void LoadModel(int Index, ContentManager theContentManager, string theAssetName, float Scale)
        {
            ModelList[Index].LoadContent(theContentManager, theAssetName);
            ModelList[Index].Scale = Scale;
        }
        public static void Init(ContentManager Manager)
        {

        }
        public static void RenderModel(int ModelIndex, Vector3 Position, Matrix Rotation)
        {
            bool SetRenderSpace = false;
            for (int i = 0; i < MAXRENDERENTITYS && !SetRenderSpace; i++)
            {
                if (!RenderEntity3dList[i].Active)
                {
                    RenderEntity3dList[i].Active = true;
                    RenderEntity3dList[i].Alpha = 1;
                    RenderEntity3dList[i].Position = Position;
                    RenderEntity3dList[i].Rotation = Rotation;
                    RenderEntity3dList[i].ModelID = ModelIndex;
                    RenderEntity3dList[i].Scale = 1;
                    SetRenderSpace = true;
                }
            }
        }
        public static void ClearRenderList()
        {
            for (int i = 0; i < MAXRENDERENTITYS; i++)
            {
                RenderEntity3dList[i].Active = false;
            }
        }

        public static void Draw(GraphicsDevice Device, GameTime Time)
        {
            // Calculate the camera matrices.
            Viewport viewport = Device.Viewport;
            Matrix view = Camera.GetViewMatrix();
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    (float)viewport.Width / (float)viewport.Height,
                                                                    1,
                                                                    10000);
            
            Matrix worldMatrix;
            for (int i = 0; i < MAXRENDERENTITYS; i++)
            {
                if (RenderEntity3dList[i].Active)
                {
                    worldMatrix = RenderEntity3dList[i].Rotation * Matrix.CreateTranslation(RenderEntity3dList[i].Position);
                    ModelList[RenderEntity3dList[i].ModelID].Draw(worldMatrix, 
                        view, projection,RenderEntity3dList[i].Scale, RenderEntity3dList[i].Alpha);
                }
            }
        }

        public static void Draw(GraphicsDevice Device, Matrix view, Matrix projection, Matrix Adjust, GameTime Time)
        {
            // Calculate the camera matrices.
            Viewport viewport = Device.Viewport;
            //Matrix view = BoatGameTestbed.mCamera.View;
            //Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            //                                                        (float)viewport.Width / (float)viewport.Height,
            //                                                        3,
            //                                                        10000);

            Matrix worldMatrix;

            //effect.Parameters["View"].SetValue(view);
            //effect.Parameters["Projection"].SetValue(projection);

            for (int i = 0; i < MAXRENDERENTITYS; i++)
            {
                if (RenderEntity3dList[i].Active)
                {
                    RenderEntity3dList[i].Rotation.Translation = Vector3.Zero;
                    worldMatrix = RenderEntity3dList[i].Rotation * Matrix.CreateTranslation(RenderEntity3dList[i].Position);
                    ModelList[RenderEntity3dList[i].ModelID].DrawWithCull(worldMatrix * Adjust,
                        view, projection, 1, 1);
                }
            }
        }

    }
}
