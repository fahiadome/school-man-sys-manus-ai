import { useState } from 'react';
import { Menu, X, ChevronDown, ChevronRight } from 'lucide-react';

interface SidebarItem {
  title: string;
  id: string;
  subItems?: SidebarItem[];
}

interface LayoutProps {
  children: React.ReactNode;
}

const sidebarItems: SidebarItem[] = [
  {
    title: 'Introduction',
    id: 'introduction',
    subItems: [
      { title: 'Purpose', id: 'purpose' },
      { title: 'Background', id: 'background' },
      { title: 'Scope', id: 'scope' },
      { title: 'Document Conventions', id: 'document-conventions' },
      { title: 'Stakeholders', id: 'stakeholders' },
      { title: 'References', id: 'references' }
    ]
  },
  {
    title: 'System Overview',
    id: 'system-overview',
    subItems: [
      { title: 'System Vision', id: 'system-vision' },
      { title: 'System Objectives', id: 'system-objectives' },
      { title: 'Success Criteria', id: 'success-criteria' },
      { title: 'System Context', id: 'system-context' }
    ]
  },
  {
    title: 'Functional Requirements',
    id: 'functional-requirements',
    subItems: [
      { title: 'User Management', id: 'user-management' },
      { title: 'Student Information', id: 'student-information' },
      { title: 'Teacher Management', id: 'teacher-management' },
      { title: 'Curriculum Management', id: 'curriculum-management' },
      { title: 'Communication', id: 'communication' },
      { title: 'Financial Management', id: 'financial-management' },
      { title: 'Resource Management', id: 'resource-management' },
      { title: 'Reporting & Analytics', id: 'reporting-analytics' },
      { title: 'Multi-Tenant Administration', id: 'multi-tenant-admin' },
      { title: 'Mobile & Offline Access', id: 'mobile-offline' }
    ]
  },
  {
    title: 'Non-Functional Requirements',
    id: 'non-functional-requirements',
    subItems: [
      { title: 'Performance', id: 'performance' },
      { title: 'Security', id: 'security' },
      { title: 'Usability', id: 'usability' },
      { title: 'Compatibility', id: 'compatibility' },
      { title: 'Reliability', id: 'reliability' },
      { title: 'Localization', id: 'localization' },
      { title: 'Infrastructure', id: 'infrastructure' }
    ]
  },
  {
    title: 'Implementation',
    id: 'implementation',
    subItems: [
      { title: 'Development Approach', id: 'development-approach' },
      { title: 'Deployment Strategy', id: 'deployment-strategy' },
      { title: 'Maintenance & Support', id: 'maintenance-support' },
      { title: 'Risk Management', id: 'risk-management' }
    ]
  },
  {
    title: 'Appendices',
    id: 'appendices',
    subItems: [
      { title: 'Glossary', id: 'glossary' },
      { title: 'References', id: 'appendix-references' },
      { title: 'Document History', id: 'document-history' }
    ]
  }
];

const Layout = ({ children }: LayoutProps) => {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [expandedItems, setExpandedItems] = useState<string[]>([]);

  const toggleSidebar = () => {
    setIsSidebarOpen(!isSidebarOpen);
  };

  const toggleExpand = (id: string) => {
    if (expandedItems.includes(id)) {
      setExpandedItems(expandedItems.filter(item => item !== id));
    } else {
      setExpandedItems([...expandedItems, id]);
    }
  };

  const renderSidebarItems = (items: SidebarItem[], level = 0) => {
    return items.map((item) => (
      <div key={item.id} className={`ml-${level * 4}`}>
        <div 
          className={`flex items-center py-2 px-4 hover:bg-gray-100 cursor-pointer ${level === 0 ? 'font-semibold' : ''}`}
          onClick={() => item.subItems ? toggleExpand(item.id) : null}
        >
          <a href={`#${item.id}`} className="flex-grow" onClick={(e) => {
            if (item.subItems) {
              e.preventDefault();
              toggleExpand(item.id);
            }
            if (window.innerWidth < 768) {
              setIsSidebarOpen(false);
            }
          }}>
            {item.title}
          </a>
          {item.subItems && (
            expandedItems.includes(item.id) ? 
              <ChevronDown size={16} /> : 
              <ChevronRight size={16} />
          )}
        </div>
        {item.subItems && expandedItems.includes(item.id) && (
          <div className="ml-4">
            {renderSidebarItems(item.subItems, level + 1)}
          </div>
        )}
      </div>
    ));
  };

  return (
    <div className="flex flex-col min-h-screen">
      {/* Header */}
      <header className="bg-blue-800 text-white shadow-md">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <div className="flex items-center">
            <button 
              className="md:hidden mr-4 focus:outline-none" 
              onClick={toggleSidebar}
              aria-label="Toggle navigation"
            >
              {isSidebarOpen ? <X size={24} /> : <Menu size={24} />}
            </button>
            <h1 className="text-xl font-bold">
              <a href="/">Multi-Tenant School Management System</a>
            </h1>
          </div>
          <div className="hidden md:flex space-x-4">
            <a href="#introduction" className="hover:text-blue-200">Introduction</a>
            <a href="#system-overview" className="hover:text-blue-200">Overview</a>
            <a href="#functional-requirements" className="hover:text-blue-200">Requirements</a>
            <a href="#implementation" className="hover:text-blue-200">Implementation</a>
          </div>
        </div>
      </header>

      <div className="flex flex-1">
        {/* Sidebar - Mobile (Overlay) */}
        <aside 
          className={`fixed inset-0 bg-white z-20 transform ${
            isSidebarOpen ? 'translate-x-0' : '-translate-x-full'
          } md:hidden transition-transform duration-300 ease-in-out overflow-y-auto`}
          style={{ width: '80%', maxWidth: '300px' }}
        >
          <div className="p-4 border-b">
            <div className="flex justify-between items-center">
              <h2 className="font-bold text-lg">Contents</h2>
              <button onClick={toggleSidebar} aria-label="Close sidebar">
                <X size={24} />
              </button>
            </div>
          </div>
          <nav className="py-4">
            {renderSidebarItems(sidebarItems)}
          </nav>
        </aside>

        {/* Sidebar - Desktop (Fixed) */}
        <aside className="hidden md:block w-64 bg-gray-50 border-r overflow-y-auto">
          <div className="p-4 border-b">
            <h2 className="font-bold text-lg">Contents</h2>
          </div>
          <nav className="py-4">
            {renderSidebarItems(sidebarItems)}
          </nav>
        </aside>

        {/* Main Content */}
        <main className="flex-1 p-4 md:p-8 overflow-y-auto">
          {/* Overlay for mobile when sidebar is open */}
          {isSidebarOpen && (
            <div 
              className="fixed inset-0 bg-black bg-opacity-50 z-10 md:hidden" 
              onClick={toggleSidebar}
            ></div>
          )}
          
          <div className="container mx-auto max-w-4xl">
            {children}
          </div>
        </main>
      </div>

      {/* Footer */}
      <footer className="bg-gray-100 border-t py-6">
        <div className="container mx-auto px-4 text-center text-gray-600">
          <p>Product Requirement Document for Ghanaian Basic and JHS Schools</p>
          <p className="mt-2 text-sm">© {new Date().getFullYear()} - All Rights Reserved</p>
        </div>
      </footer>
    </div>
  );
};

export default Layout;
