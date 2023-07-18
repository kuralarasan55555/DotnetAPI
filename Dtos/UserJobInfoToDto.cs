namespace DotnetAPI.Models
{
    public partial class UserJobInfoToDto
    {
        public string JobTitle {get; set;}
        public string Department {get; set;}
        public  UserJobInfoToDto()
        {
            if(Department==null){
                Department="";
            }
            if(JobTitle==null){
                JobTitle="";
            }
           
        }
    }
}