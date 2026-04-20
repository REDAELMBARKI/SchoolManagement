import { useState } from "react";
import { Link } from "react-router-dom";
import { role } from "@/lib/data";

const Navbar = () => {
  const userRole = role;
  const [showDropdown, setShowDropdown] = useState(false);
  
  // Get today's date and season
  const today = new Date();
  const dateStr = today.toLocaleDateString('en-US', { 
    weekday: 'short', 
    month: 'short', 
    day: 'numeric' 
  });
  
  const getSeason = (date: Date) => {
    const month = date.getMonth();
    if (month >= 2 && month <= 4) return 'Spring';
    if (month >= 5 && month <= 7) return 'Summer';
    if (month >= 8 && month <= 10) return 'Fall';
    return 'Winter';
  };
  
  const season = getSeason(today);

  return (
    <div className="flex items-center justify-between p-4">
      <div className="hidden md:flex items-center gap-4">
        <div className="flex items-center gap-2 text-xs rounded-full ring-[1.5px] ring-gray-300 px-2">
          <img src="/search.png" alt="" width={14} height={14} />
          <input
            type="text"
            placeholder="Search..."
            className="w-[200px] p-2 bg-transparent outline-none"
          />
        </div>
        <div className="text-sm text-gray-600">
          <span className="font-medium">{dateStr}</span>
          <span className="ml-2 text-xs text-gray-500">({season})</span>
        </div>
      </div>
      
      <div className="flex items-center gap-6 justify-end w-full">
        {/* Plus dropdown button */}
        <div className="relative">
          <button
            onClick={() => setShowDropdown(!showDropdown)}
            className="bg-lamaYellow rounded-full w-8 h-8 flex items-center justify-center hover:bg-lamaYellow/80 transition-colors"
          >
            <img src="/plus.png" alt="Add" width={16} height={16} />
          </button>
          
          {showDropdown && (
            <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg border border-gray-200 z-50">
              <div className="py-1">
                {role === "admin" && (
                  <>
                    <Link
                      to="/list/intakes/new"
                      className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                      onClick={() => setShowDropdown(false)}
                    >
                      Add Intake
                    </Link>
                    <Link
                      to="/list/students/new"
                      className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                      onClick={() => setShowDropdown(false)}
                    >
                      Add Student
                    </Link>
                    <Link
                      to="/list/teachers/new"
                      className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                      onClick={() => setShowDropdown(false)}
                    >
                      Add Teacher
                    </Link>
                  </>
                )}
                {(role === "admin" || role === "teacher") && (
                  <>
                    <Link
                      to="/list/exams/new"
                      className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                      onClick={() => setShowDropdown(false)}
                    >
                      Add Exam
                    </Link>
                  </>
                )}
              </div>
            </div>
          )}
        </div>
        
        <div className="bg-white rounded-full w-7 h-7 flex items-center justify-center cursor-pointer">
          <img src="/message.png" alt="" width={20} height={20} />
        </div>
        <div className="bg-white rounded-full w-7 h-7 flex items-center justify-center cursor-pointer relative">
          <img src="/announcement.png" alt="" width={20} height={20} />
          <div className="absolute -top-3 -right-3 w-5 h-5 flex items-center justify-center bg-purple-500 text-white rounded-full text-xs">
            1
          </div>
        </div>
        <div className="flex flex-col">
          <span className="text-xs leading-3 font-medium">John Doe</span>
          <span className="text-[10px] text-gray-500 text-right">
            {userRole}
          </span>
        </div>
        <img
          src="/avatar.png"
          alt=""
          width={36}
          height={36}
          className="rounded-full"
        />
      </div>
    </div>
  );
};

export default Navbar;
