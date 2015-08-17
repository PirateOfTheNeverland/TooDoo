<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="TooDooSvc" generation="1" functional="0" release="0" Id="731590e4-f324-4204-8dfd-1dfed0665d89" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="TooDooSvcGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="TooDooWebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/TooDooSvc/TooDooSvcGroup/LB:TooDooWebRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="TooDooWebRole:AzureConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/TooDooSvc/TooDooSvcGroup/MapTooDooWebRole:AzureConnectionString" />
          </maps>
        </aCS>
        <aCS name="TooDooWebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/TooDooSvc/TooDooSvcGroup/MapTooDooWebRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:TooDooWebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapTooDooWebRole:AzureConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRole/AzureConnectionString" />
          </setting>
        </map>
        <map name="MapTooDooWebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="TooDooWebRole" generation="1" functional="0" release="0" software="C:\Users\EyeLESS\Documents\GitHub\TooDoo\TooDooSvc\csx\Release\roles\TooDooWebRole" entryPoint="base\x86\WaHostBootstrapper.exe" parameters="base\x86\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="AzureConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;TooDooWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;TooDooWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="TooDooWebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="TooDooWebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="TooDooWebRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="20ca9abe-1868-45f8-923b-a0e37d0ac6fd" ref="Microsoft.RedDog.Contract\ServiceContract\TooDooSvcContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="f676abb0-8d40-4ed9-ba53-4bdfeb657f7a" ref="Microsoft.RedDog.Contract\Interface\TooDooWebRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/TooDooSvc/TooDooSvcGroup/TooDooWebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>