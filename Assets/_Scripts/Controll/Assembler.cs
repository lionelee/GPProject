using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour
{
    //GameObject sbond;
    GameObject catom;
    Dictionary<GameObject, GameObject> sbonds;
    bool grabbed;

    void Start()
    {
        sbonds = new Dictionary<GameObject, GameObject>();
        catom = null;
    }

    public void SetGrabbed()
    {
        grabbed = true;
    }

    public void ResetGrabbed()
    {
        grabbed = false;
    }

    public void Connect()
    {
        if (catom != null)
        {
            foreach(GameObject obj in sbonds.Keys)
            {
                Destroy(sbonds[obj]);
            }

            // get molecules and atoms
            Molecule m1 = catom.GetComponentInParent<Molecule>();
            Molecule m2 = GetComponentInParent<Molecule>();
            Atom a1 = catom.GetComponent<Atom>();
            Atom a2 = GetComponent<Atom>();

            if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
            {
                return;
            }

            //calculate expected position of this atom
            string key;
            if (a1.Symbol.Length <= a2.Symbol.Length && a1.Symbol[0] < a2.Symbol[0])
            {
                key = a1.Symbol + a2.Symbol;
            }
            else
            {
                key = a2.Symbol + a1.Symbol;
            }
            float length = Config.BondLengthTable[key];
            Vector3 pos = catom.transform.position;
            Vector3 angle = a1.getAngle();
            Vector3 expectedPos = angle * length + pos;

            //draw bond
            GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
            GameObject bond = Instantiate(prefebMole);

            bond.transform.position = Vector3.Lerp(pos, expectedPos, 0.5f);
            Vector3 scale = prefebMole.transform.lossyScale;
            scale.y = length * 0.5f;
            bond.transform.localScale = scale;
            bond.transform.LookAt(expectedPos);
            bond.transform.Rotate(new Vector3(90, 0, 0));
            bond.transform.parent = catom.transform.parent;

            //set abstract bond
            Bond b = bond.AddComponent<Bond>();
            b.A1 = a1.Id;
            b.A2 = a2.Id;
            b.Type = BondType.SINGLE;
            a1.addBond(b);
            a2.addBond(b);

            //move and rotate this atom and the molecule to match bond angle vector
            transform.parent.transform.position += expectedPos - transform.position;

            Vector3 vec1 = catom.transform.TransformDirection(angle);
            Vector3 vec2 = transform.TransformDirection(a2.getAngle());
            Vector3 axis = Vector3.Cross(vec1, vec2);

            float an = Vector3.Angle(vec1, vec2);
            if(axis == Vector3.zero)
            {
                axis = new Vector3(0, 1, 0);
                an = 180.0f;
            }
            transform.parent.transform.RotateAround(transform.position, axis, an);
            
            //merge two molecules into one
            Molecule m = catom.transform.parent.gameObject.GetComponent<Molecule>();
            GameObject parent = transform.parent.gameObject;
            foreach (Transform child in transform.parent.transform)
            {
                child.parent = catom.transform.parent;
                if (child.gameObject.tag != "Bond")
                {
                    child.gameObject.GetComponent<Atom>().Id = m.CurrentAtomId++;
                }
            }

            Destroy(parent);
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        if (!grabbed)
            return;


        DeleteSbond(collider.gameObject);

        print(collider.gameObject);
        print("trigger enter");
        gameObject.transform.parent.GetComponent<MoleculesAction>().SetConnectableAtom(gameObject);


        Atom a1 = collider.gameObject.GetComponent<Atom>();
        Atom a2 = GetComponent<Atom>();

        if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
        {
            Debug.Log("can not be connected");
            return;
        }

        // show bond cloud be connected
        Vector3 pos1 = collider.gameObject.transform.position;
        Vector3 pos2 = transform.position;

        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        GameObject bond = Instantiate(prefebMole);
        bond.GetComponent<Renderer>().material.color = Color.yellow;
        bond.transform.position = Vector3.Lerp(pos1, pos2, 0.5f);
        float distance = Vector3.Distance(pos1, pos2);
        Vector3 scale = prefebMole.transform.lossyScale;
        scale.y = distance * 0.5f;
        bond.transform.localScale = scale;

        bond.transform.LookAt(pos2);
        bond.transform.Rotate(new Vector3(90, 0, 0));


        sbonds.Add(collider.gameObject, bond);
        catom = collider.gameObject;
    }

    void DeleteSbond(GameObject gameObject)
    {
        //delete bonds that shown before
        print("exit");
        if (sbonds.ContainsKey(gameObject))
        {
            GameObject sbond = sbonds[gameObject];
            sbonds.Remove(gameObject);
            Destroy(sbond);
        }

    }

    void OnTriggerExit(Collider collider)
    {
        //delete bonds that shown before
        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        print("exit trigger");
        if (!grabbed)
            return;
        
        DeleteSbond(collider.gameObject);

        if (catom == gameObject)
        {
            catom = null;
        }

    }
}
