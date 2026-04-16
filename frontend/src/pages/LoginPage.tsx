import { useNavigate } from "react-router-dom";

const LoginPage = () => {
  const navigate = useNavigate();

  return (
    <div className="h-screen flex items-center justify-center bg-lamaSkyLight">
      <div className="bg-white p-12 rounded-md shadow-2xl flex flex-col gap-4">
        <h1 className="text-xl font-bold flex items-center gap-2">
          <img src="/logo.png" alt="" width={24} height={24} />
          SchooLama
        </h1>
        <h2 className="text-gray-400">Offline Frontend Mode</h2>
        <button
          type="button"
          onClick={() => {
            localStorage.setItem("demoRole", "admin");
            navigate("/admin");
          }}
          className="bg-blue-500 text-white my-1 rounded-md text-sm p-[10px]"
        >
          Enter as Admin
        </button>
        <button
          type="button"
          onClick={() => {
            localStorage.setItem("demoRole", "student");
            navigate("/student");
          }}
          className="bg-green-500 text-white my-1 rounded-md text-sm p-[10px]"
        >
          Enter as Student
        </button>
      </div>
    </div>
  );
};

export default LoginPage;
