using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class User
{
    public string name;
    public string score;

    public User()
    {
    }

    public User(string name, string score)
    {
        this.name = name;
        this.score = score;
    }
}

public class firebaseDB : MonoBehaviour
{
    DatabaseReference reference;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://puzzle2048-56b51.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        User user = new User("혜림이", "바보");
        string json = JsonUtility.ToJson(user);
        //reference.Child("users").SetRawJsonValueAsync(json);




        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/users/0/name"] = "test";
        childUpdates["/users/0/score"] = "11111";


        reference.UpdateChildrenAsync(childUpdates);




        FirebaseDatabase.DefaultInstance
     .GetReference("users") // 읽어올 데이터 이름
     .GetValueAsync().ContinueWith(task =>
     {
         if (task.IsFaulted)
         {
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;

             Debug.Log(snapshot.Value.ToString());
             // DataSnapshot 타입에 저장된 값 불러오기
             foreach (var item in snapshot.Children)
             {
                 Debug.Log(item.Child("email").Value);
                 Debug.Log(item.Child("score").Value);

             }
         }
     });

        OnClickMaxScores();

    }

   public void OnClickMaxScores() 
   { 
       const int MaxScoreRecordCount = 5; 
       int score = 66; 
       string email = "testEmail"; 


        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference; 
        mDatabaseRef.Child("top5scores").RunTransaction(mutableData => { 
            List<object> leaders = mutableData.Value as List<object>; 


            if (leaders == null)  
            { 
                leaders = new List<object>(); 
            }  
            else if (mutableData.ChildrenCount >= MaxScoreRecordCount)  
            { 
                long minScore = long.MaxValue; 
                object minVal = null; 
                foreach (var child in leaders)  
                { 
                    if (!(child is Dictionary<string, object>))  
                        continue; 
                    long childScore = (long)((Dictionary<string, object>)child)["score"]; 
                    if (childScore < minScore)  
                    { 
                        minScore = childScore; 
                        minVal = child; 
                    } 
                } 
                if (minScore > score)  
                { 
                    // The new score is lower than the existing 5 scores, abort. 
                    return TransactionResult.Abort(); 
                } 


                // Remove the lowest score. 
                leaders.Remove(minVal); 
            } 


            Dictionary<string, object> entryValues = new Dictionary<string, object>();             
            entryValues.Add("score", score); 
            entryValues.Add("email", email); 
            leaders.Add(entryValues); 


            mutableData.Value = leaders; 
            return TransactionResult.Success(mutableData); 
        }).ContinueWith( 
            task => 
            { 
                Debug.Log(string.Format("OnClickMaxScores::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted)); 
            } 
        ); 
    } 

}
