import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { role } from "@/lib/data";

type MenuItem = {
  icon: string;
  label: string;
  href: string;
  visible: string[];
};

type MenuGroup = {
  icon: string;
  label: string;
  visible: string[];
  children: MenuItem[];
};

type MenuSection = {
  title: string;
  items: (MenuItem | MenuGroup)[];
};

const isMenuGroup = (item: MenuItem | MenuGroup): item is MenuGroup =>
  "children" in item;

const menuItems: MenuSection[] = [
  {
    title: "MENU",
    items: [
      {
        icon: "/setting.png",
        label: "Theme",
        href: "/theme",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/home.png",
        label: "Home",
        href: "/",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/teacher.png",
        label: "Teachers",
        href: "/list/teachers",
        visible: ["admin", "teacher"],
      },
      {
        icon: "/student.png",
        label: "Students",
        href: "/list/students",
        visible: ["admin", "teacher"],
      },
      {
        icon: "/parent.png",
        label: "Parents",
        href: "/list/parents",
        visible: ["admin", "teacher"],
      },
      // ── Intakes dropdown group ───────────────────────────────────────
      {
        icon: "/student.png",
        label: "Intakes",
        visible: ["admin", "teacher"],
        children: [
          {
            icon: "/lesson.png",
            label: "All Intakes",
            href: "/list/intakes",
            visible: ["admin", "teacher"],
          },
          {
            icon: "/create.png",
            label: "New Intake",
            href: "/list/intakes/new",
            visible: ["admin"],
          },
        ],
      },
      // ────────────────────────────────────────────────────────────────
      {
        icon: "/lesson.png",
        label: "Schedule Builder",
        href: "/schedule-builder",
        visible: ["admin", "teacher"],
      },
      {
        icon: "/subject.png",
        label: "Subjects",
        href: "/list/subjects",
        visible: ["admin"],
      },
      {
        icon: "/class.png",
        label: "Classes",
        href: "/list/classes",
        visible: ["admin", "teacher"],
      },
      {
        icon: "/lesson.png",
        label: "Lessons",
        href: "/list/lessons",
        visible: ["admin", "teacher"],
      },
      {
        icon: "/exam.png",
        label: "Exams",
        href: "/list/exams",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/assignment.png",
        label: "Assignments",
        href: "/list/assignments",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/result.png",
        label: "Results",
        href: "/list/results",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/attendance.png",
        label: "Attendance",
        href: "/list/attendance",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/calendar.png",
        label: "Events",
        href: "/list/events",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/message.png",
        label: "Messages",
        href: "/list/messages",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/announcement.png",
        label: "Announcements",
        href: "/list/announcements",
        visible: ["admin", "teacher", "student", "parent"],
      },
    ],
  },
  {
    title: "OTHER",
    items: [
      {
        icon: "/profile.png",
        label: "Profile",
        href: "/profile",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/setting.png",
        label: "Settings",
        href: "/settings",
        visible: ["admin", "teacher", "student", "parent"],
      },
      {
        icon: "/logout.png",
        label: "Logout",
        href: "/logout",
        visible: ["admin", "teacher", "student", "parent"],
      },
    ],
  },
];

const Menu = () => {
  const userRole = role as string;
  const location = useLocation();
  const [openGroups, setOpenGroups] = useState<Record<string, boolean>>({
    Intakes: location.pathname.startsWith("/list/intakes"),
  });

  const toggleGroup = (label: string) => {
    setOpenGroups((prev) => ({ ...prev, [label]: !prev[label] }));
  };

  return (
    <div className="mt-4 text-sm">
      {menuItems.map((section) => (
        <div className="flex flex-col gap-2" key={section.title}>
          <span className="hidden lg:block text-gray-400 font-light my-4">
            {section.title}
          </span>

          {section.items.map((item) => {
            if (!item.visible.includes(userRole)) return null;

            // ── Dropdown group ──────────────────────────────────────────
            if (isMenuGroup(item)) {
              const isOpen = !!openGroups[item.label];
              const isActive = item.children.some((c) =>
                location.pathname.startsWith(c.href)
              );

              return (
                <div key={item.label}>
                  {/* Group trigger */}
                  <button
                    onClick={() => toggleGroup(item.label)}
                    className={`w-full flex items-center justify-center lg:justify-start gap-4 py-2 md:px-2 rounded-md transition-colors
                      ${isActive ? "bg-lamaSkyLight text-gray-700" : "text-gray-500 hover:bg-lamaSkyLight"}`}
                  >
                    <img src={item.icon} alt="" width={20} height={20} />
                    <span className="hidden lg:block flex-1 text-left">
                      {item.label}
                    </span>
                    {/* Chevron — only visible on lg+ */}
                    <span
                      className={`hidden lg:block text-gray-400 text-xs transition-transform duration-200 ${
                        isOpen ? "rotate-180" : ""
                      }`}
                      style={{ display: "inline-block" }}
                    >
                      ▾
                    </span>
                  </button>

                  {/* Sub-items */}
                  {isOpen && (
                    <div className="flex flex-col gap-1 mt-1 lg:pl-4 border-l-2 border-lamaSkyLight ml-2 lg:ml-6">
                      {item.children.map((child) => {
                        if (!child.visible.includes(userRole)) return null;
                        const childActive = location.pathname === child.href;
                        return (
                          <Link
                            key={child.label}
                            to={child.href}
                            className={`flex items-center justify-center lg:justify-start gap-3 py-1.5 px-2 rounded-md transition-colors text-xs
                              ${childActive ? "bg-lamaPurpleLight text-gray-700 font-medium" : "text-gray-500 hover:bg-lamaSkyLight"}`}
                          >
                            <img
                              src={child.icon}
                              alt=""
                              width={14}
                              height={14}
                            />
                            <span className="hidden lg:block">
                              {child.label}
                            </span>
                          </Link>
                        );
                      })}
                    </div>
                  )}
                </div>
              );
            }

            // ── Regular link ────────────────────────────────────────────
            const isActive = location.pathname === item.href;
            return (
              <Link
                to={item.href}
                key={item.label}
                className={`flex items-center justify-center lg:justify-start gap-4 py-2 md:px-2 rounded-md transition-colors
                  ${isActive ? "bg-lamaSkyLight text-gray-700" : "text-gray-500 hover:bg-lamaSkyLight"}`}
              >
                <img src={item.icon} alt="" width={20} height={20} />
                <span className="hidden lg:block">{item.label}</span>
              </Link>
            );
          })}
        </div>
      ))}
    </div>
  );
};

export default Menu;
