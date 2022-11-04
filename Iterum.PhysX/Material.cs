using Iterum.Physics;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum;

public class Material : IMaterial
{
    public readonly float staticFriction;
    public readonly float dynamicFriction;
    public readonly float restitution;
        
    private long nRef;
        
    public Material(float staticFriction = 0.5f, float dynamicFriction = 0.5f, float restitution = 0.5f)
    {
        this.staticFriction = staticFriction;
        this.dynamicFriction = dynamicFriction;
        this.restitution = restitution;
            
        nRef = API.createMaterial(staticFriction, dynamicFriction, restitution);
    }

    public void Destroy()
    {
        API.cleanupMaterial(nRef);
    }
        
    public long GetInternal()
    {
        return nRef;
    }
}