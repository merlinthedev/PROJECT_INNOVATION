using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace shared {
    /**
     * Classes that extend ASerializable can (de)serialize themselves into/out of a Packet instance. 
     * See the classes in the protocol package for an example. 
     * This base class provides a ToString method for simple (and slow) debugging.
     */
    public interface ISerializable {
        public void Serialize(Packet packet);
        public void Deserialize(Packet packet);
        
    }
}
