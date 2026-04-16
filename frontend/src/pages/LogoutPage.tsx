import { useNavigate } from "react-router-dom";
import { useEffect } from "react";

export default function LogoutPage() {
  const navigate = useNavigate();

  useEffect(() => {
    navigate("/", { replace: true });
  }, [navigate]);

  return (
    <div className="p-8 m-4 bg-white rounded-md">
      <p className="text-gray-600">Returning to login…</p>
    </div>
  );
}
