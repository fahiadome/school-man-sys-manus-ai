// HomePage component for PRD website

const HomePage = () => {
  return (
    <div className="prose prose-lg max-w-none">
      <section id="introduction">
        <h1 className="text-3xl font-bold mb-6">Product Requirement Document (PRD)</h1>
        <h2 className="text-2xl font-bold mb-4">Multi-Tenant School Management System for Ghanaian Basic and JHS Schools</h2>
        
        <div id="purpose" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">1.1 Purpose</h3>
          <p>
            This Product Requirement Document (PRD) outlines the comprehensive requirements, architecture, and features for a multi-tenant school management system specifically designed for Ghanaian Basic and Junior High Schools (JHS). The document serves as a definitive reference for stakeholders involved in the development, implementation, and deployment of the system, ensuring alignment with the unique educational, administrative, and infrastructural context of Ghana.
          </p>
        </div>
        
        <div id="background" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">1.2 Background</h3>
          <p>
            Ghana's educational system follows a 6-3-3-4 structure, with Basic Education encompassing 6 years of primary education and 3 years of Junior High School. This foundational education is critical to the nation's development goals and faces several challenges, including administrative inefficiencies, resource constraints, infrastructure limitations, and connectivity issues, particularly in rural areas.
          </p>
          <p>
            Traditional school management approaches in Ghana often rely on manual, paper-based processes that are time-consuming, error-prone, and limit data-driven decision making. While some urban schools have adopted standalone digital solutions, these systems typically operate in isolation, preventing efficient resource sharing and standardization across the educational landscape.
          </p>
          <p>
            A multi-tenant school management system offers significant potential to address these challenges by providing a unified platform that can accommodate diverse school environments while optimizing resource utilization, standardizing processes, and facilitating collaboration. By leveraging cloud technologies with robust offline capabilities, such a system can function effectively even in areas with limited connectivity, bringing modern management tools to schools throughout Ghana.
          </p>
        </div>
        
        <div id="scope" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">1.3 Scope</h3>
          <p>
            This PRD covers the requirements, architecture, and features for a comprehensive multi-tenant school management system designed specifically for Ghanaian Basic and JHS schools. The system will address the following key areas:
          </p>
          <ul className="list-disc pl-6 mb-4">
            <li>Student information management and academic tracking</li>
            <li>Teacher and staff administration</li>
            <li>Curriculum and academic management aligned with Ghanaian standards</li>
            <li>Communication and collaboration among stakeholders</li>
            <li>Financial management and resource allocation</li>
            <li>Reporting and analytics for data-driven decision making</li>
            <li>Multi-tenant administration and customization</li>
            <li>Integration with external educational systems and services</li>
          </ul>
          <p>
            The system will be designed to function effectively across diverse infrastructure environments, from well-connected urban schools to remote rural institutions with limited connectivity. It will support both English and major Ghanaian languages, accommodate local administrative practices, and comply with relevant educational policies and data protection regulations.
          </p>
        </div>
        
        <div id="document-conventions" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">1.4 Document Conventions</h3>
          <p>Throughout this document, the following conventions are used:</p>
          <ul className="list-disc pl-6 mb-4">
            <li>"The system" refers to the multi-tenant school management system being specified</li>
            <li>"Tenant" refers to an individual school or educational institution using the system</li>
            <li>"User" refers to any stakeholder interacting with the system, including administrators, teachers, students, and parents</li>
            <li>"GES" refers to the Ghana Education Service</li>
            <li>"Basic Education" encompasses kindergarten (2 years), primary school (6 years), and JHS (3 years)</li>
            <li>"Must" indicates a mandatory requirement</li>
            <li>"Should" indicates a recommended but not mandatory requirement</li>
          </ul>
        </div>
        
        <div id="stakeholders" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">1.5 Stakeholders</h3>
          <p>The following stakeholders have been identified for the multi-tenant school management system:</p>
          
          <h4 className="font-semibold mt-4 mb-2">Primary Stakeholders:</h4>
          <ul className="list-disc pl-6 mb-4">
            <li>School administrators and headmasters</li>
            <li>Teachers and educational staff</li>
            <li>Students</li>
            <li>Parents and guardians</li>
            <li>District and regional education officers</li>
            <li>Ministry of Education and Ghana Education Service officials</li>
          </ul>
          
          <h4 className="font-semibold mt-4 mb-2">Secondary Stakeholders:</h4>
          <ul className="list-disc pl-6 mb-4">
            <li>System administrators and IT support personnel</li>
            <li>Educational content providers</li>
            <li>Third-party service providers (payment processors, SMS providers)</li>
            <li>Educational researchers and policy makers</li>
            <li>Community organizations and NGOs involved in education</li>
          </ul>
        </div>
        
        <div id="references" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">1.6 References</h3>
          <p>This PRD draws upon the following authoritative sources:</p>
          <ol className="list-decimal pl-6 mb-4">
            <li>Ghana Education Service Curriculum Framework for Basic Schools</li>
            <li>Ministry of Education policies and guidelines for Basic Education</li>
            <li>Basic Education Certificate Examination (BECE) requirements and standards</li>
            <li>Ghana's ICT in Education Policy</li>
            <li>Ghana Data Protection Act</li>
            <li>Educational Management Information System (EMIS) standards</li>
          </ol>
        </div>
      </section>
      
      <section id="system-overview" className="mt-12">
        <h2 className="text-2xl font-bold mb-4">2. System Overview and Objectives</h2>
        
        <div id="system-vision" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">2.1 System Vision</h3>
          <p>
            The multi-tenant school management system aims to transform educational administration in Ghanaian Basic and JHS schools by providing a unified, efficient, and accessible platform that addresses local challenges while promoting standardization, resource optimization, and data-driven decision making. The system will bridge administrative gaps between urban and rural schools, support national educational policies, and enhance collaboration among all stakeholders.
          </p>
        </div>
        
        <div id="system-objectives" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">2.2 System Objectives</h3>
          <p>The primary objectives of the multi-tenant school management system are to:</p>
          
          <ol className="list-decimal pl-6 mb-4">
            <li>
              <strong>Improve Administrative Efficiency:</strong> Streamline administrative processes, reduce paperwork, and automate routine tasks to allow school staff to focus more on educational activities.
            </li>
            <li>
              <strong>Enhance Educational Outcomes:</strong> Support teaching and learning processes through better resource management, curriculum alignment, and performance tracking.
            </li>
            <li>
              <strong>Optimize Resource Utilization:</strong> Enable efficient allocation and sharing of limited resources across schools through centralized management and visibility.
            </li>
            <li>
              <strong>Bridge Urban-Rural Divides:</strong> Provide equitable access to modern management tools for all schools, regardless of location or infrastructure limitations.
            </li>
            <li>
              <strong>Support Data-Driven Decision Making:</strong> Generate actionable insights through comprehensive reporting and analytics at school, district, and national levels.
            </li>
            <li>
              <strong>Facilitate Communication:</strong> Enhance collaboration and information sharing among administrators, teachers, students, parents, and educational authorities.
            </li>
            <li>
              <strong>Ensure Standardization with Flexibility:</strong> Implement consistent processes aligned with national standards while accommodating local needs and practices.
            </li>
            <li>
              <strong>Promote Transparency and Accountability:</strong> Provide clear visibility into school operations, resource allocation, and academic performance.
            </li>
          </ol>
        </div>
        
        <div id="success-criteria" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">2.3 Success Criteria</h3>
          <p>The success of the multi-tenant school management system will be measured by the following criteria:</p>
          
          <ol className="list-decimal pl-6 mb-4">
            <li>
              <strong>Adoption Rate:</strong> Percentage of target schools successfully implementing and actively using the system.
            </li>
            <li>
              <strong>Administrative Time Savings:</strong> Reduction in time spent on administrative tasks by school staff.
            </li>
            <li>
              <strong>Data Completeness and Accuracy:</strong> Improvement in the completeness, timeliness, and accuracy of educational data.
            </li>
            <li>
              <strong>Stakeholder Satisfaction:</strong> Positive feedback from administrators, teachers, parents, and educational authorities.
            </li>
            <li>
              <strong>Resource Optimization:</strong> Measurable improvements in resource allocation and utilization.
            </li>
            <li>
              <strong>System Availability:</strong> Percentage of time the system is available and functional, including in areas with connectivity challenges.
            </li>
            <li>
              <strong>Reporting Compliance:</strong> Ability to generate all required reports for GES and other educational authorities accurately and on time.
            </li>
            <li>
              <strong>Cost Efficiency:</strong> Reduction in overall administrative costs relative to traditional management approaches.
            </li>
          </ol>
        </div>
        
        <div id="system-context" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">2.4 System Context</h3>
          <p>
            The multi-tenant school management system will operate within the broader Ghanaian educational ecosystem, interacting with various external systems and stakeholders:
          </p>
          
          <ol className="list-decimal pl-6 mb-4">
            <li>
              <strong>Ghana Education Service Systems:</strong> For policy updates, curriculum standards, and reporting requirements.
            </li>
            <li>
              <strong>District and Regional Education Offices:</strong> For oversight, resource allocation, and performance monitoring.
            </li>
            <li>
              <strong>National Examination Systems:</strong> For BECE registration, preparation, and results processing.
            </li>
            <li>
              <strong>Payment Systems:</strong> For fee collection, financial transactions, and payroll processing.
            </li>
            <li>
              <strong>Communication Services:</strong> For SMS notifications, email communications, and other messaging channels.
            </li>
            <li>
              <strong>Educational Content Providers:</strong> For curriculum resources, learning materials, and assessment tools.
            </li>
            <li>
              <strong>Community and Parent Organizations:</strong> For engagement, feedback, and support activities.
            </li>
          </ol>
          <p>
            The system must be designed to integrate effectively with these external entities while maintaining appropriate data security and privacy controls.
          </p>
        </div>
      </section>
      
      <section id="functional-requirements" className="mt-12">
        <h2 className="text-2xl font-bold mb-4">3. Functional Requirements</h2>
        
        <div id="user-management" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">3.1 User Management and Authentication</h3>
          
          <h4 className="font-semibold mt-4 mb-2">3.1.1 User Types and Roles</h4>
          <p>The system must support the following user types, each with appropriate role-based permissions:</p>
          
          <ol className="list-decimal pl-6 mb-4">
            <li>
              <strong>System Administrators:</strong> Responsible for overall system management, tenant provisioning, and system-wide configurations.
            </li>
            <li>
              <strong>School Administrators:</strong> Headmasters, assistant headmasters, and administrative staff responsible for school-level management.
            </li>
            <li>
              <strong>Teachers:</strong> Teaching staff responsible for academic activities, student assessment, and classroom management.
            </li>
            <li>
              <strong>Students:</strong> Learners enrolled in the school, with access to appropriate educational resources and personal information.
            </li>
            <li>
              <strong>Parents/Guardians:</strong> Family members responsible for students, with access to their children's information and school communications.
            </li>
            <li>
              <strong>District/Regional Officers:</strong> Educational authorities with oversight responsibilities for multiple schools.
            </li>
            <li>
              <strong>Support Staff:</strong> Non-teaching staff such as accountants, librarians, and facility managers.
            </li>
          </ol>
          
          <p>Each role must have configurable permissions that can be adapted to match the specific administrative hierarchy and practices of Ghanaian schools.</p>
          
          <h4 className="font-semibold mt-4 mb-2">3.1.2 Authentication and Security</h4>
          <p>The system must provide secure authentication mechanisms, including:</p>
          <ul className="list-disc pl-6 mb-4">
            <li>Username and password authentication with strong password policies</li>
            <li>Multi-factor authentication for administrative and sensitive functions</li>
            <li>Session management with appropriate timeouts</li>
            <li>Account lockout after multiple failed login attempts</li>
          </ul>
          
          <p>The system must support role-based access control with granular permissions:</p>
          <ul className="list-disc pl-6 mb-4">
            <li>Function-level permissions (view, create, edit, delete)</li>
            <li>Data-level permissions (which records a user can access)</li>
            <li>Temporal permissions (time-limited access for specific functions)</li>
          </ul>
        </div>
        
        <div id="student-information" className="mb-8">
          <h3 className="text-xl font-semibold mb-3">3.2 Student Information Management</h3>
          
          <h4 className="font-semibold mt-4 mb-2">3.2.1 Student Records</h4>
          <p>The system must maintain comprehensive student profiles including:</p>
          <ul className="list-disc pl-6 mb-4">
            <li>Personal information (name, date of birth, gender, contact details)</li>
            <li>Family information (parents/guardians, siblings, emergency contacts)</li>
            <li>Academic history and current enrollment status</li>
            <li>Health information (medical conditions, immunizations, allergies)</li>
            <li>Special needs and accommodations</li>
            <li>Behavioral records and disciplinary history</li>
          </ul>
          
          <h4 className="font-semibold mt-4 mb-2">3.2.2 Enrollment and Registration</h4>
          <p>The system must support the complete student enrollment lifecycle:</p>
          <ul className="list-disc pl-6 mb-4">
            <li>Application and admission processes</li>
            <li>Registration for academic terms</li>
            <li>Class and section assignment</li>
            <li>Promotion and graduation</li>
            <li>Transfer between schools within the system</li>
            <li>Withdrawal and alumni tracking</li>
          </ul>
        </div>
        
        <div className="text-center my-8">
          <p className="italic text-gray-600">
            This is a condensed preview of the PRD. The full document contains detailed specifications for all system components.
          </p>
          <a href="#" className="text-blue-600 hover:underline">
            View more sections
          </a>
        </div>
      </section>
      
      <section id="system-architecture" className="mt-12">
        <h2 className="text-2xl font-bold mb-4">System Architecture</h2>
        
        <div className="mb-8">
          <h3 className="text-xl font-semibold mb-3">Multi-Tenant Architecture</h3>
          <p>
            The system employs a hybrid cloud-based architecture with strong offline capabilities, designed specifically to function in environments with limited or intermittent connectivity. This approach combines the benefits of centralized data management and resource sharing with the resilience needed for operation in areas with infrastructure challenges.
          </p>
          
          <div className="bg-blue-50 border-l-4 border-blue-500 p-4 my-4">
            <h4 className="font-semibold mb-2">Key Architectural Components:</h4>
            <ul className="list-disc pl-6">
              <li>Multi-Tenant Data Layer with hybrid approach</li>
              <li>Modular and service-oriented Application Layer</li>
              <li>Responsive Presentation Layer with offline capabilities</li>
              <li>Integration Layer for external systems</li>
              <li>Synchronization and Offline Operation Engine</li>
            </ul>
          </div>
          
          <p>
            The architecture incorporates several features to ensure scalability and performance across diverse environments, including horizontal scalability, multi-level caching, and bandwidth-efficient synchronization protocols optimized for high-latency, low-bandwidth connections.
          </p>
        </div>
      </section>
      
      <section id="implementation" className="mt-12">
        <h2 className="text-2xl font-bold mb-4">Implementation Considerations</h2>
        
        <div className="mb-8">
          <p>
            The implementation of the multi-tenant school management system will follow an iterative, user-centered approach with phased deployment to accommodate the diverse infrastructure environments in Ghanaian schools.
          </p>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">
            <div className="bg-gray-50 p-4 rounded shadow-sm">
              <h4 className="font-semibold mb-2">Development Approach</h4>
              <ul className="list-disc pl-6">
                <li>Phased implementation of core functionality</li>
                <li>Continuous user involvement from Ghanaian educators</li>
                <li>Agile methodology to accommodate evolving requirements</li>
                <li>Early prototype testing in diverse school environments</li>
              </ul>
            </div>
            
            <div className="bg-gray-50 p-4 rounded shadow-sm">
              <h4 className="font-semibold mb-2">Deployment Strategy</h4>
              <ul className="list-disc pl-6">
                <li>Limited pilot in representative schools</li>
                <li>Phased rollout with lessons learned integration</li>
                <li>Comprehensive training and support</li>
                <li>Data migration from existing systems</li>
                <li>Connectivity-appropriate deployment patterns</li>
              </ul>
            </div>
          </div>
        </div>
      </section>
      
      <section id="conclusion" className="mt-12 mb-8">
        <h2 className="text-2xl font-bold mb-4">Conclusion</h2>
        <p>
          The multi-tenant school management system described in this PRD represents a comprehensive solution to the administrative challenges faced by Ghanaian Basic and JHS schools. By combining modern cloud technologies with robust offline capabilities and contextual adaptations, the system aims to transform educational administration across Ghana's diverse school environments.
        </p>
        <p className="mt-4">
          Through careful implementation and ongoing support, this system has the potential to significantly improve administrative efficiency, enhance resource utilization, and support data-driven decision making, ultimately contributing to better educational outcomes for Ghanaian students.
        </p>
      </section>
    </div>
  );
};

export default HomePage;
